using System;

namespace Mergen.Core.Options
{
    public class EmailVerificationOptions
    {
        public string LinkFormat { get; set; }
        public string RedirectUrl { get; set; }
        public TimeSpan ExpiresAfter { get; set; }
    }
}