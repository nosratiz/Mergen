using Mergen.Game.Api.ViewModels.Errors;

namespace Mergen.Game.Api.CustomResult
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