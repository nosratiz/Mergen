namespace Mergen.Api.Core.Exceptions
{
    public class UnprocessableEntityException : ApiException
    {
        public UnprocessableEntityException(string errorCode, string errorDescription) : base(422, errorCode,
            errorDescription)
        {
        }
    }
}