using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.Managers.Base;
using Mergen.Core.QueryProcessing;

namespace Mergen.Core.Managers
{
    public class FileManager : EntityManagerBase<AccountFile>
    {
        public FileManager(DbContextFactory dbContextFactory, QueryProcessor queryProcessor) : base(dbContextFactory,
            queryProcessor)
        {
        }
    }
}