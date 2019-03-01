using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
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

        public override async Task DeleteAsync(Question entity, CancellationToken cancellationToken = default)
        {
            using (var dbc = CreateDbContext())
            {
                dbc.QuestionCategories.RemoveRange(dbc.QuestionCategories.Where(q => q.QuestionId == entity.Id));
                dbc.Questions.Remove(entity);
                await dbc.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task UpdateQuestionCategories(Question question, IEnumerable<long> categoryIds,
            CancellationToken cancellationToken = default)
        {
            using (var dbc = CreateDbContext())
            {
                if (categoryIds == null || !categoryIds.Any())
                {
                    dbc.QuestionCategories.RemoveRange(dbc.QuestionCategories.Where(q => q.QuestionId == question.Id));
                    return;
                }

                var currentQuestionCategories = await dbc.QuestionCategories.Where(q => q.QuestionId == question.Id)
                    .ToListAsync(cancellationToken);

                var newQuestionCategories = new List<QuestionCategory>();
                foreach (var categoryId in categoryIds)
                {
                    var existingItem = currentQuestionCategories.FirstOrDefault(q => q.CategoryId == categoryId);
                    if (existingItem == null)
                    {
                        newQuestionCategories.Add(new QuestionCategory
                        {
                            QuestionId = question.Id,
                            CategoryId = categoryId
                        });
                    }
                }

                foreach (var currentQuestionCategory in currentQuestionCategories)
                {
                    if (!categoryIds.Contains(currentQuestionCategory.CategoryId))
                    {
                        dbc.QuestionCategories.Remove(currentQuestionCategory);
                    }
                }

                foreach (var questionCategory in newQuestionCategories)
                {
                    dbc.QuestionCategories.Add(questionCategory);
                }

                await dbc.SaveChangesAsync(cancellationToken);
            }
        }

        public void SetQuestionCategory(Question question, long categoryId)
        {
            question.QuestionCategories.Add(new QuestionCategory
            {
                Question = question,
                CategoryId = categoryId
            });

            question.CategoryIdsCache = categoryId.ToString();
        }
    }
}