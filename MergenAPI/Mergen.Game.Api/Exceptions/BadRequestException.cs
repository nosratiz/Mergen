using Mergen.Game.Api.Exceptions;

namespace MMergen.Game.ApiExceptions
{
    public class BadRequestException : ApiException
    {
        public BadRequestException(string errorCode, string errorDescription) : base(400, errorCode, errorDescription)
        {
        }
    }
}