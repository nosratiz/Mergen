using Mergen.Admin.Api.ViewModels.Errors;

namespace Mergen.Admin.Api.CustomResult
{
    public class BadRequestResult : ErrorResult
    {
        public BadRequestResult(string errorCode, string errorDescription)
            : base(400,
                new ErrorViewModel
                {
                    ErrorCode = errorCode,
                    ErrorDescription = errorDescription
                })
        {
        }
    }
}