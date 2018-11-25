using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Mergen.Core.QueryProcessing
{
    public class InputProcessor
    {
        private readonly PropertyCache _propertyCache;

        public InputProcessor(PropertyCache propertyCache)
        {
            _propertyCache = propertyCache;
        }

        public QueryInputModel ParseQuery(Type type, string queryString)
        {
            var queryParameters = ParseQueryParameters(queryString);

            return new QueryInputModel
            {
                FilterParameters = ParseFilters(type, queryParameters),
                SortParameters = ParseSortParameters(type, queryParameters),
                PaginationParameter = ParsePaginationParameter(type, queryParameters)
            };
        }

        public static QueryParameter[] ParseQueryParameters(string queryString)
        {
            if (queryString.StartsWith("?"))
                queryString = queryString.Substring(1);

            var queryParameters = queryString.Split(new[] {'&'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(q =>
                {
                    var keyValue = q.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);
                    return new QueryParameter(keyValue[0], HttpUtility.UrlDecode(keyValue[1]));
                }).ToArray();
            return queryParameters;
        }

        public FilterParameter[] ParseFilters(Type type, QueryParameter[] queryParameters)
        {
            var filters = new List<FilterParameter>();

            var properties = _propertyCache.GetProperties(type);
            foreach (var propertyInfo in properties)
            {
                var filter = queryParameters.FirstOrDefault(q =>
                    string.Equals(q.Key, propertyInfo.Name, StringComparison.OrdinalIgnoreCase));

                if (filter != null)
                {
                    var opStr = queryParameters.FirstOrDefault(q =>
                        string.Equals($"{filter.Key}_op", q.Key, StringComparison.OrdinalIgnoreCase));

                    var op = Op.Equals;
                    if (opStr != null)
                        op = ParseOperator(opStr.Value);

                    filters.Add(new FilterParameter(filter.Key, op, filter.Value, propertyInfo));
                }

                var nullFilter = queryParameters.FirstOrDefault(q =>
                    q.Key.Equals($"{propertyInfo.Name}_op", StringComparison.OrdinalIgnoreCase) &&
                    (q.Value.Equals("isnull", StringComparison.OrdinalIgnoreCase) ||
                     q.Value.Equals("notnull", StringComparison.OrdinalIgnoreCase)));

                if (nullFilter != null)
                    filters.Add(new FilterParameter(propertyInfo.Name, ParseOperator(nullFilter.Value),
                        nullFilter.Value, propertyInfo));
            }

            return filters.ToArray();
        }

        public static Op ParseOperator(string op)
        {
            switch (op.ToLower(CultureInfo.InvariantCulture))
            {
                case "eq":
                    return Op.Equals;
                case "neq":
                case "ne":
                    return Op.NotEquals;
                case "gt":
                    return Op.GreaterThan;
                case "gte":
                case "gteq":
                    return Op.GreaterThanOrEqualTo;
                case "lt":
                    return Op.LessThan;
                case "lte":
                case "lteq":
                    return Op.LessThanOrEqualTo;
                case "in":
                    return Op.In;
                case "bw":
                case "between":
                    return Op.Between;
                case "contains":
                case "has":
                    return Op.Contains;
                case "startswith":
                case "sw":
                    return Op.StartsWith;
                case "isnull":
                    return Op.IsNull;
                case "notnull":
                    return Op.NotNull;
                default:
                    throw new InvalidEnumArgumentException($"Operator {op} is undefined.");
            }
        }

        public SortParameter[] ParseSortParameters(Type type, QueryParameter[] queryParameters)
        {
            var sortParam =
                queryParameters.FirstOrDefault(q => string.Equals(q.Key, "_sort", StringComparison.OrdinalIgnoreCase));
            if (sortParam != null)
            {
                var sortFields = sortParam.Value.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                var propInfos = _propertyCache.GetProperties(type);
                var sortParams = new List<SortParameter>();
                foreach (var sortField in sortFields)
                {
                    var fieldName = sortField;

                    var isDesc = fieldName.StartsWith("-");
                    if (isDesc)
                        fieldName = fieldName.Substring(1);

                    var propInfo = propInfos.FirstOrDefault(q =>
                        string.Equals(q.Name, fieldName, StringComparison.OrdinalIgnoreCase));
                    if (propInfo != null)
                    {
                        sortParams.Add(new SortParameter(propInfo.Name, isDesc));
                    }
                }

                if (sortParams.Any())
                    return sortParams.ToArray();
            }

            return null;
        }

        public PaginationParameter ParsePaginationParameter(Type type, QueryParameter[] queryParameters)
        {
            var pageNumberParam = queryParameters.FirstOrDefault(q =>
                string.Equals(q.Key, "_pageNumber", StringComparison.OrdinalIgnoreCase));
            if (pageNumberParam != null)
            {
                var pageSizeParam = queryParameters.FirstOrDefault(q =>
                    string.Equals(q.Key, "_pageSize", StringComparison.OrdinalIgnoreCase));
                if (pageSizeParam != null)
                    return new PaginationParameter(int.Parse(pageSizeParam.Value), int.Parse(pageNumberParam.Value));
            }

            return null;
        }
    }
}