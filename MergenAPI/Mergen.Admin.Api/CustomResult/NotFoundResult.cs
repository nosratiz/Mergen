using Mergen.Admin.Api.ViewModels.Errors;

namespace Mergen.Admin.Api.CustomResult
{
    public class NotFoundResult : ErrorResult
    {
        public NotFoundResult(string errorCode = "not_found", string errorDescription = null)
            : base(404,
                new ErrorViewModel
                {
                    ErrorCode = errorCode,
                    ErrorDescription = errorDescription
                })
        {
        }
    }
}