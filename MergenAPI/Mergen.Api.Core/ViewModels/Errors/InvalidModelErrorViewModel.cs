﻿using System.Collections.Generic;

namespace Mergen.Api.Core.ViewModels.Errors
{
    public class InvalidModelErrorViewModel : ErrorViewModel
    {
        public IEnumerable<ErrorViewModel> Errors { get; set; }
    }
}