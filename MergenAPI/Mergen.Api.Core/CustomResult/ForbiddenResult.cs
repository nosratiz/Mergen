using Mergen.Api.Core.ViewModels.Errors;

namespace Mergen.Api.Core.CustomResult
{
    public class ForbiddenResult : ErrorResult
    {
        public ForbiddenResult(string errorCode = "forbidden", string errorDescription = null) : base(403,
            new ErrorViewModel
            {
                ErrorCode = errorCode,
                ErrorDescription = errorDescription
            })
        {
        }
    }
}