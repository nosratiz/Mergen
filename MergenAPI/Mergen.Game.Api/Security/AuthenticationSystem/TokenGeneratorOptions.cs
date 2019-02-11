using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Mergen.Game.Api.Security.AuthenticationSystem
{
    public class TokenGeneratorOptions
    {
        public string TokenIssuer { get; set; }
        public TimeSpan Expiretion { get; set; }
        public string TokenAudience { get; set; }
        public string TokenSecretKey { get; set; }
        public IEnumerable<Claim> Claims { get; set; }
    }
}