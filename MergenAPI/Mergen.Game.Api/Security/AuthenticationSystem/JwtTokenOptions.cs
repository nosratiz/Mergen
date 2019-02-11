using System;

namespace Mergen.Game.Api.Security.AuthenticationSystem
{
    public class JwtTokenOptions
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecretKey { get; set; }
        public TimeSpan Expiretion { get; set; }
    }
}