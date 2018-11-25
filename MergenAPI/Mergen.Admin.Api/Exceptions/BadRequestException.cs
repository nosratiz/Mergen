namespace Mergen.Admin.Api.Exceptions
{
    public class BadRequestException : ApiException
    {
        public BadRequestException(string errorCode, string errorDescription) : base(400, errorCode, errorDescription)
        {
        }
    }
}