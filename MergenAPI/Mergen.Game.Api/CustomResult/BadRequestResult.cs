using Mergen.Game.Api.ViewModels.Errors;

namespace Mergen.Game.Api.CustomResult
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