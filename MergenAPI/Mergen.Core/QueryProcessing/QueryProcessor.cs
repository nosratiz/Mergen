using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Core.QueryProcessing
{
    public class QueryProcessor
    {
        private readonly PropertyCache _propertyCache;

        public QueryProcessor(PropertyCache propertyCache)
        {
            _propertyCache = propertyCache;
        }

        public Task<QueryResult<TEntity>> ApplyAsync<TEntity>(IQueryable<TEntity> entities, QueryInputModel queryModel,
            CancellationToken cancellationToken)
        {
            var filters = queryModel.FilterParameters;
            var pagination = queryModel.PaginationParameter;
            var sorts = queryModel.SortParameters;

            return ApplyAsync(entities, filters, sorts, pagination, cancellationToken);
        }

        public async Task<QueryResult<TEntity>> ApplyAsync<TEntity>(IQueryable<TEntity> entities, FilterParameter[] filters,
            SortParameter[] sorts, PaginationParameter pagination,
            CancellationToken cancellationToken)
        {
            if (filters != null && filters.Length > 0)
            {
                var propreties = _propertyCache.GetProperties<TEntity>();
                var parameterExpression = Expression.Parameter(typeof(TEntity), "e");
                Expression outerExpression = null;
                foreach (var filter in filters)
                {
                    var property =
                        propreties.FirstOrDefault(q =>
                            string.Equals(q.Name, filter.FieldName, StringComparison.OrdinalIgnoreCase));

                    if (property == null)
                        throw new ArgumentException($"Property for Filter.Field({filter.FieldName}) not found.");

                    var converter = TypeDescriptor.GetConverter(property.PropertyType);

                    dynamic propertyValue = Expression.PropertyOrField(parameterExpression, property.Name);

                    Expression innerExpression = null;
                    if (filter.Op == Op.In)
                    {
                        foreach (var value in filter.Values)
                        {
                            dynamic constantVal = GetConstantVal<TEntity>(converter, value, property);

                            Expression filterValue = Expression.Constant(constantVal, property.PropertyType);

                            var filterExpression = GetBinaryExpression(Op.Equals, filterValue, propertyValue);

                            innerExpression = innerExpression == null
                                ? filterExpression
                                : Expression.OrElse(innerExpression, filterExpression);
                        }
                    }
                    else if (filter.Op == Op.Between)
                    {
                        dynamic leftConstantVal = GetConstantVal<TEntity>(converter, filter.Values[0], property);
                        Expression leftFilterValue = Expression.Constant(leftConstantVal, property.PropertyType);

                        dynamic rightConstantVal = GetConstantVal<TEntity>(converter, filter.Values[1], property);
                        Expression rightFilterValue = Expression.Constant(rightConstantVal, property.PropertyType);

                        innerExpression = Expression.And(
                            GetBinaryExpression(Op.GreaterThanOrEqualTo, leftFilterValue, propertyValue),
                            GetBinaryExpression(Op.LessThanOrEqualTo, rightFilterValue, propertyValue)
                        );
                    }
                    else if (filter.Op == Op.IsNull || filter.Op == Op.NotNull)
                    {
                        innerExpression = GetBinaryExpression(filter.Op, null, propertyValue);
                    }
                    else
                    {
                        dynamic constantVal = GetConstantVal<TEntity>(converter, filter.Values[0], property);
                        Expression filterValue = Expression.Constant(constantVal, property.PropertyType);
                        innerExpression = GetBinaryExpression(filter.Op, filterValue, propertyValue);
                    }

                    if (outerExpression == null)
                    {
                        outerExpression = innerExpression;
                    }
                    else
                    {
                        outerExpression = Expression.AndAlso(outerExpression, innerExpression);
                    }
                }

                entities = outerExpression == null
                    ? entities
                    : entities.Where(Expression.Lambda<Func<TEntity, bool>>(outerExpression, parameterExpression));
            }

            if (sorts != null && sorts.Length > 0)
            {
                var useThenBy = false;
                foreach (var sortParameter in sorts)
                {
                    entities = OrderByDynamic(entities, sortParameter.Field, sortParameter.Desc, useThenBy);
                    useThenBy = true;
                }
            }

            if (pagination != null)
            {
                var totalCount = entities.Count();
                var result = await entities.Skip((pagination.PageNumber - 1) * pagination.PageSize)
                    .Take(pagination.PageSize).ToArrayAsync(cancellationToken);

                return new QueryResult<TEntity>
                {
                    Data = result,
                    TotalCount = totalCount,
                    PageSize = pagination.PageSize,
                    PageNumber = pagination.PageNumber
                };
            }

            var data = await entities.ToArrayAsync(cancellationToken);
            return new QueryResult<TEntity>
            {
                Data = data,
                TotalCount = data.Length
            };
        }

        private static object GetConstantVal<TEntity>(TypeConverter converter, string value, PropertyInfo property)
        {
            return converter.CanConvertFrom(typeof(string))
                ? converter.ConvertFrom(value)
                : Convert.ChangeType(value, property.PropertyType);
        }

        private static Expression GetBinaryExpression(Op op, dynamic filterValue, dynamic propertyValue)
        {
            switch (op)
            {
                case Op.Equals:
                    return Expression.Equal(propertyValue, filterValue);
                case Op.NotEquals:
                    return Expression.NotEqual(propertyValue, filterValue);
                case Op.GreaterThan:
                    return Expression.GreaterThan(propertyValue, filterValue);
                case Op.LessThan:
                    return Expression.LessThan(propertyValue, filterValue);
                case Op.GreaterThanOrEqualTo:
                    return Expression.GreaterThanOrEqual(propertyValue, filterValue);
                case Op.LessThanOrEqualTo:
                    return Expression.LessThanOrEqual(propertyValue, filterValue);
                case Op.Contains:
                    return Expression.Call(propertyValue,
                        typeof(string).GetMethods()
                            .First(m => m.Name == "Contains" && m.GetParameters().Length == 1),
                        filterValue);
                case Op.StartsWith:
                    return Expression.Call(propertyValue,
                        typeof(string).GetMethods()
                            .First(m => m.Name == "StartsWith" && m.GetParameters().Length == 1),
                        filterValue);
                case Op.IsNull:
                    return Expression.Equal(propertyValue, Expression.Constant(null, typeof(object)));
                case Op.NotNull:
                    return Expression.NotEqual(propertyValue, Expression.Constant(null, typeof(object)));
                default:
                    return Expression.Equal(propertyValue, filterValue);
            }
        }

        public static IQueryable<TEntity> OrderByDynamic<TEntity>(IQueryable<TEntity> source, string orderByProperty,
            bool desc, bool useThenBy)
        {
            string command = desc
                ? (useThenBy ? "ThenByDescending" : "OrderByDescending")
                : (useThenBy ? "ThenBy" : "OrderBy");
            var type = typeof(TEntity);
            var property = type.GetProperty(orderByProperty);
            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExpression = Expression.Lambda(propertyAccess, parameter);
            var resultExpression = Expression.Call(typeof(Queryable), command, new[] {type, property.PropertyType},
                source.Expression, Expression.Quote(orderByExpression));
            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
}