using System.Collections.Generic;

namespace Mergen.Admin.Api.ViewModels.Errors
{
    public class InvalidModelErrorViewModel : ErrorViewModel
    {
        public IEnumerable<ErrorViewModel> Errors { get; set; }
    }
}