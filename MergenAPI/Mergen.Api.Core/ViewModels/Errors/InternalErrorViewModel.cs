namespace Mergen.Api.Core.ViewModels.Errors
{
    public class InternalErrorViewModel : ErrorViewModel
    {
        public string TraceId { get; set; }

        public InternalErrorViewModel(string errorDescription, string traceId = null)
        {
            ErrorCode = "internal_error";
            ErrorDescription = errorDescription;
            TraceId = traceId;
        }
    }
}