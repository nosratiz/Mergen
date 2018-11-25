using System;

namespace Mergen.Core.Options
{
    public class ResetPasswordOptions
    {
        public string LinkFormat { get; set; }
        public TimeSpan ExpiresAfter { get; set; }
    }
}