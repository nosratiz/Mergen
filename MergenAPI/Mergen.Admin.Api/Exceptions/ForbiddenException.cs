namespace Mergen.Admin.Api.Exceptions
{
    public class ForbiddenException : ApiException
    {
        public ForbiddenException(string errorCode, string errorDescription) : base(403, errorCode, errorDescription)
        {
        }
    }
}