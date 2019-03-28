using System.Linq;
using Mergen.Api.Core.ViewModels.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Mergen.Admin.Api.Helpers
{
    public static class ResultHelpers
    {
        public static ActionResult ToUnprocessableEntityResult(this ModelStateDictionary modelState)
        {
            return new UnprocessableEntityObjectResult(new InvalidModelErrorViewModel
            {
                ErrorCode = "invalid_model",
                Errors = modelState.SelectMany(m => m.Value.Errors.Select(err => new FieldError
                {
                    ErrorCode = "field_error",
                    FieldName = m.Key,
                    ErrorDescription = err.ErrorMessage
                }))
            });
        }
    }
}