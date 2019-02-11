using Microsoft.AspNetCore.Mvc;

namespace Mergen.Game.Api.ViewModels.Errors
{
    public class ErrorViewModel : ActionResult
    {
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
    }
}