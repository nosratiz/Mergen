using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Mergen.Admin.Api.Exceptions;
using Mergen.Admin.Api.Security.AuthorizationSystem;
using Mergen.Core.QueryProcessing;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Mergen.Admin.Api.QueryProcessing
{
    public class QueryModelBinder : IModelBinder
    {
        private readonly InputProcessor _inputProcessor;

        public QueryModelBinder(InputProcessor inputProcessor)
        {
            _inputProcessor = inputProcessor;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var filterModelType = bindingContext.ModelType.GetGenericArguments()[0];
            var queryString = bindingContext.ActionContext.HttpContext.Request.QueryString.Value;

            var queryModel = _inputProcessor.ParseQuery(filterModelType, queryString);
            foreach (var filterParameter in queryModel.FilterParameters)
            {
                var validOperators = filterParameter.PropertyInfo.GetCustomAttribute<ValidOperatorsAttribute>();

                if (validOperators != null)
                {
                    if (validOperators.IgnoreValidityPermission != null &&
                        bindingContext.HttpContext.User?.HasPermission(validOperators.IgnoreValidityPermission) == true)
                        continue;

                    if (validOperators.Operators.All(validOp => validOp != filterParameter.Op))
                        throw new ForbiddenException("forbidden_operator",
                            $"Operator {filterParameter.Op} is invalid for filter parameter {filterParameter.FieldName}");
                }
            }

            var model = (QueryInputModel) Activator.CreateInstance(bindingContext.ModelType);

            model.FilterParameters = queryModel.FilterParameters;
            model.PaginationParameter = queryModel.PaginationParameter;
            model.SortParameters = queryModel.SortParameters;
            bindingContext.Result = ModelBindingResult.Success(model);
            return Task.CompletedTask;
        }
    }
}