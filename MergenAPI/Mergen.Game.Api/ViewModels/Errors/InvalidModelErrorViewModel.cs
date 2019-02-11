using System.Collections.Generic;

namespace Mergen.Game.Api.ViewModels.Errors
{
    public class InvalidModelErrorViewModel : ErrorViewModel
    {
        public IEnumerable<ErrorViewModel> Errors { get; set; }
    }
}