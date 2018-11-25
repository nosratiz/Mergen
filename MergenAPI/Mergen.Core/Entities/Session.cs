using System;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class Session : Entity
    {
        public string AccessToken { get; set; }
        public long AccountId { get; set; }
        public DateTime CreationDateTime { get; set; }
        public int StateId { get; set; }
    }
}