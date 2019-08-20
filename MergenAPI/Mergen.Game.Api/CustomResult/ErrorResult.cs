using Mergen.Api.Core.ViewModels;
using Mergen.Api.Core.ViewModels.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Mergen.Game.Api.CustomResult
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