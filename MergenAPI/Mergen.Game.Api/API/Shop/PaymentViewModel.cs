using System.Collections.Generic;

namespace Mergen.Game.Api.API.Shop
{
    public class PaymentViewModel
    {
        public string PaygateUrl { get; set; }
        public IDictionary<string, string> PaygateParameters { get; set; }
        public string CallbackUrl { get; set; }
    }
}