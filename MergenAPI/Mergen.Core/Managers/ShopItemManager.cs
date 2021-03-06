using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.Managers.Base;
using Mergen.Core.QueryProcessing;

namespace Mergen.Core.Managers
{
    public class ShopItemManager : EntityManagerBase<ShopItem>
    {
        public ShopItemManager(DbContextFactory dbContextFactory, QueryProcessor queryProcessor) : base(dbContextFactory, queryProcessor)
        {
        }
    }
}