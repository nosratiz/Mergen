using Mergen.Admin.Api.ViewModels;
using Mergen.Admin.Api.ViewModels.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Mergen.Admin.Api.CustomResult
{
    public class ErrorResult : ObjectResult
    {
        public ErrorResult(int statusCode, ErrorViewModel errorViewModel) : base(
            ApiResultViewModel<object>.FromError(errorViewModel))
        {
            StatusCode = statusCode;
        }
    }
}