using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Mergen.Core.QueryProcessing;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Mergen.Api.Core.QueryProcessing
{
    public class QueryModelOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var queryType = typeof(QueryInputModel);
            var parmInfos = context.MethodInfo.GetParameters();
            foreach (var parameterInfo in parmInfos)
            {
                if (queryType.IsAssignableFrom(parameterInfo.ParameterType))
                {
                    operation.Description = "this thing is for queries";
                    var filterModelProperties = parameterInfo.ParameterType.GetGenericArguments()[0].GetProperties();
                    operation.Parameters.Remove(operation.Parameters.FirstOrDefault(p => p.Name == parameterInfo.Name));
                    operation.Parameters = operation.Parameters.Where(p => !(p.Name.Contains("ModelType") ||
                                                                             p.Name == "FilterParameters" ||
                                                                             p.Name == "SortParameters" ||
                                                                             p.Name.Contains("PaginationParameter"))
                    ).ToList();
                    foreach (var p in filterModelProperties)
                    {
                        var requiredAttr = p.GetCustomAttribute(typeof(RequiredAttribute));
                        operation.Parameters.Add(new NonBodyParameter
                        {
                            Type = p.PropertyType.Name,
                            Name = p.Name,
                            In = "query",
                            Required = requiredAttr != null
                        });
                        operation.Parameters.Add(new NonBodyParameter
                        {
                            Type = "Operator",
                            Name = $"{p.Name}_op",
                            In = "query"
                        });
                    }

                    operation.Parameters.Add(new NonBodyParameter
                    {
                        Name = "_sort",
                        Type = "string",
                        In = "query"
                    });

                    operation.Parameters.Add(new NonBodyParameter
                    {
                        Name = "_pageSize",
                        Type = "Int32",
                        In = "query"
                    });

                    operation.Parameters.Add(new NonBodyParameter
                    {
                        Name = "_pageNumber",
                        Type = "Int32",
                        In = "query"
                    });
                }
            }
        }
    }
}