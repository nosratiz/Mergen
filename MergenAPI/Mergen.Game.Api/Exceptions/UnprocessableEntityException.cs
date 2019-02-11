using Mergen.Game.Api.Exceptions;

namespace MMergen.Game.ApiExceptions
{
    public class UnprocessableEntityException : ApiException
    {
        public UnprocessableEntityException(string errorCode, string errorDescription) : base(422, errorCode,
            errorDescription)
        {
        }
    }
}