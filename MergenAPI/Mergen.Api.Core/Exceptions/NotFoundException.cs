namespace Mergen.Api.Core.Exceptions
{
    public class NotFoundException : ApiException
    {
        public NotFoundException(string errorCode = "not_found", string errorDescription = null) : base(404, errorCode, errorDescription)
        {
        }
    }
}