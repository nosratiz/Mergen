using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class Chat : Entity
    {
        public long SenderAccountId { get; set; }
        public long ReceiverAccountId { get; set; }
        public string MessageText { get; set; }
        public DateTime DateTime { get; set; }
    }
}