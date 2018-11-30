using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Mergen.Admin.Api.Helpers;
using Mergen.Admin.Api.ViewModels;
using Mergen.Core.Entities;
using Mergen.Core.Managers;
using Mergen.Core.QueryProcessing;
using Microsoft.AspNetCore.Mvc;

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
            return OkData(data);
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
                Difficulty = inputModel.Difficulty
            };

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                item = await _questionManager.SaveAsync(item, cancellationToken);
                await _questionManager.UpdateQuestionCategories(item,
                    inputModel.CategoryIds.Select(cid => cid.ToLong()), cancellationToken);

                transaction.Complete();
            }

            return OkData(item);
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

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                item = await _questionManager.SaveAsync(item, cancellationToken);
                await _questionManager.UpdateQuestionCategories(item,
                    inputModel.CategoryIds.Select(cid => cid.ToLong()), cancellationToken);

                transaction.Complete();
            }

            item = await _questionManager.SaveAsync(item, cancellationToken);

            return OkData(item);
        }

        [HttpDelete]
        [Route("questions/{id}")]
        public async Task<ActionResult<ApiResultViewModel<QuestionViewModel>>> DeleteAsync([FromRoute] string id,
            CancellationToken cancellationToken)
        {
            var item = await _questionManager.GetByIdAsyncThrowNotFoundIfNotExists(id, cancellationToken);
            await _questionManager.DeleteAsync(item, cancellationToken);

            return OkData(item);
        }
    }
}