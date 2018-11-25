using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.Managers.Base;
using Mergen.Core.QueryProcessing;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Core.Managers
{
    public class QuestionManager : EntityManagerBase<Question>
    {
        public QuestionManager(DbContextFactory dbContextFactory, QueryProcessor queryProcessor) : base(dbContextFactory, queryProcessor)
        {
        }

        public async Task UpdateQuestionCategories(Question question, IEnumerable<long> categoryIds,
            CancellationToken cancellationToken = default)
        {
            using (var dbc = CreateDbContext())
            {
                dbc.QuestionCategories.RemoveRange(dbc.QuestionCategories.Where(q => q.QuestionId == question.Id));

                foreach (var categoryId in categoryIds)
                {
                    dbc.QuestionCategories.Add(new QuestionCategory
                    {
                        QuestionId = question.Id,
                        CategoryId = categoryId
                    });
                }

                await dbc.SaveChangesAsync(cancellationToken);
            }
        }
    }
}