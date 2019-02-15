using System;
using Mergen.Core.Entities.Base;

namespace Mergen.Core.Entities
{
    public class FinancialTransaction : Entity
    {
        public long AccountId { get; set; }
        public Account Account { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }
}