using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Mergen.Api.Core.Helpers;
using Mergen.Api.Core.ViewModels;
using Mergen.Core.Entities;
using Mergen.Core.Managers;
using Mergen.Core.QueryProcessing;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Mergen.Admin.Api.API.Questions
{
    [ApiController]
    public class QuestionController : ApiControllerBase
    {
        private readonly QuestionManager _questionManager;

        public QuestionController(QuestionManager questionManager)
        {
            _questionManager = questionManager;
        }

        [HttpGet]
        [Route("questions")]
        public async Task<ActionResult<ApiResultViewModel<QuestionViewModel>>> GetAllAsync([FromQuery]QueryInputModel<QuestionFilterInputModel> queryInputModel, CancellationToken cancellationToken)
        {
            var data = await _questionManager.GetAllAsync(queryInputModel, cancellationToken);
            return OkData(QuestionViewModel.MapAll(data.Data), new DataMetaViewModel(data.TotalCount));
        }

        [HttpGet]
        [Route("questions/{id}")]
        public async Task<ActionResult<ApiResultViewModel<QuestionViewModel>>> GetByIdAsync(string id,
            CancellationToken cancellationToken)
        {
            var data = await _questionManager.GetByIdAsyncThrowNotFoundIfNotExists(id, cancellationToken);
            return OkData(QuestionViewModel.Map(data));
        }

        [HttpPost]
        [Route("questions")]
        public async Task<ActionResult<ApiResultViewModel<QuestionViewModel>>> CreateAsync(
            [FromBody]QuestionInputModel inputModel, CancellationToken cancellationToken)
        {
            var item = new Question
            {
                Body = inputModel.Body,
                Answer1 = inputModel.Answer1,
                Answer2 = inputModel.Answer2,
                Answer3 = inputModel.Answer3,
                Answer4 = inputModel.Answer4,
                CorrectAnswerNumber = inputModel.CorrectAnswerNumber,
                Difficulty = inputModel.Difficulty,
            };

            if (inputModel.CategoryIds != null && inputModel.CategoryIds.Any())
                item.CategoryIdsCache = string.Join(",", inputModel.CategoryIds);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                item = await _questionManager.SaveAsync(item, cancellationToken);

                if (inputModel.CategoryIds != null)
                    await _questionManager.UpdateQuestionCategories(item,
                        inputModel.CategoryIds.Select(cid => cid.ToLong()), cancellationToken);

                transaction.Complete();
            }

            return OkData(QuestionViewModel.Map(item));
        }

        [HttpPost]
        [Route("questionfiles")]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200, ValueLengthLimit = 209715200)]
        public async Task<ActionResult<ApiResultViewModel<QuestionViewModel>>> ImportQuestionsFromFile(
            [FromForm]QuestionFileImportInputModel inputModel, CancellationToken cancellationToken)
        {
            var categoryId = inputModel.CategoryId.ToLong();
            var questions = new List<Question>();

            var errors = new List<string>();

            using (var p = new ExcelPackage(inputModel.File.OpenReadStream()))
            {
                foreach (var ws in p.Workbook.Worksheets)
                {
                    var start = ws.Dimension.Start;
                    var end = ws.Dimension.End;
                    for (int i = start.Row + 1; i < end.Row; i++)
                    {
                        var question = new Question();

                        question.Body = ws.Cells[i, 2].GetValue<string>();
                        question.Answer1 = ws.Cells[i, 3].GetValue<string>();
                        question.Answer2 = ws.Cells[i, 4].GetValue<string>();
                        question.Answer3 = ws.Cells[i, 5].GetValue<string>();
                        question.Answer4 = ws.Cells[i, 6].GetValue<string>();
                        var r = ws.Cells[i, 7].GetValue<string>();

                        if (r == null)
                        {
                            errors.Add($"Answer cell is empty. row:{i}");
                            continue;
                        }

                        if (r.StartsWith("A", StringComparison.OrdinalIgnoreCase) || string.Equals(r, question.Answer1))
                        {
                            question.CorrectAnswerNumber = 1;
                        }
                        else if (r.StartsWith("B", StringComparison.OrdinalIgnoreCase) || string.Equals(r, question.Answer2))
                        {
                            question.CorrectAnswerNumber = 2;
                        }
                        else if (r.StartsWith("C", StringComparison.OrdinalIgnoreCase) || string.Equals(r, question.Answer3))
                        {
                            question.CorrectAnswerNumber = 3;
                        }
                        else if (r.StartsWith("D", StringComparison.OrdinalIgnoreCase) || string.Equals(r, question.Answer4))
                        {
                            question.CorrectAnswerNumber = 4;
                        }
                        else
                        {
                            errors.Add($"Correct answer of question not found. row:{i}");
                            continue;
                        }

                        _questionManager.SetQuestionCategory(question, categoryId);
                        question = await _questionManager.SaveAsync(question, cancellationToken);
                        questions.Add(question);
                    }
                }
            }

            return OkData(QuestionViewModel.MapAll(questions), new { Errors = errors });
        }

        [HttpPut]
        [Route("questions/{id}")]
        public async Task<ActionResult<ApiResultViewModel<QuestionViewModel>>> UpdateAsync([FromRoute] string id,
            [FromBody]QuestionInputModel inputModel, CancellationToken cancellationToken)
        {
            var item = await _questionManager.GetByIdAsyncThrowNotFoundIfNotExists(id, cancellationToken);

            item.Body = inputModel.Body;
            item.Answer1 = inputModel.Answer1;
            item.Answer2 = inputModel.Answer2;
            item.Answer3 = inputModel.Answer3;
            item.Answer4 = inputModel.Answer4;
            item.CorrectAnswerNumber = inputModel.CorrectAnswerNumber;
            item.Difficulty = inputModel.Difficulty;
            item.CategoryIdsCache = string.Join(",", inputModel.CategoryIds);

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                item = await _questionManager.SaveAsync(item, cancellationToken);

                await _questionManager.UpdateQuestionCategories(item,
                    inputModel.CategoryIds?.Select(cid => cid.ToLong()), cancellationToken);

                transaction.Complete();
            }

            item = await _questionManager.SaveAsync(item, cancellationToken);

            return OkData(QuestionViewModel.Map(item));
        }

        [HttpDelete]
        [Route("questions/{id}")]
        public async Task<ActionResult> DeleteAsync([FromRoute] string id,
            CancellationToken cancellationToken)
        {
            var item = await _questionManager.GetByIdAsyncThrowNotFoundIfNotExists(id, cancellationToken);
            await _questionManager.ArchiveAsync(item, cancellationToken);

            return Ok();
        }
    }
}