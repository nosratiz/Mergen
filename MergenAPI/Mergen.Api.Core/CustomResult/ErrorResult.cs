using Mergen.Api.Core.ViewModels;
using Mergen.Api.Core.ViewModels.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Mergen.Api.Core.CustomResult
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