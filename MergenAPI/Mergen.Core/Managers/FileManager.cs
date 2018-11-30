using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Mergen.Core.Managers.Base;
using Mergen.Core.QueryProcessing;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Core.Managers
{
    public class FileManager : EntityManagerBase<UploadedFile>
    {
        private readonly QueryProcessor _queryProcessor;

        public FileManager(DbContextFactory dbContextFactory, QueryProcessor queryProcessor) : base(dbContextFactory,
            queryProcessor)
        {
            _queryProcessor = queryProcessor;
        }

        public async Task<UploadedFile> GetFileByFileIdAsync(string fileId, CancellationToken cancellationToken)
        {
            return await FirstOrDefaultAsync(f => f.FileId == fileId, cancellationToken);
        }
    }
}