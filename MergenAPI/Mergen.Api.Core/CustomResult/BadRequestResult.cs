using Mergen.Api.Core.ViewModels.Errors;

namespace Mergen.Api.Core.CustomResult
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