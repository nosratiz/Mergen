using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Admin.Api.Helpers;
using Mergen.Api.Core.ViewModels;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Mergen.Core.Managers;
using Mergen.Core.QueryProcessing;
using Microsoft.AspNetCore.Mvc;

namespace Mergen.Admin.Api.API.Categories
{
    [ApiController]
    public class CategoryController : ApiControllerBase
    {
        private readonly CategoryManager _categoryManager;

        public CategoryController(CategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }

        [HttpGet]
        [Route("questioncategories")]
        public async Task<ActionResult<ApiResultViewModel<CategoryViewModel>>> GetAllAsync([FromQuery]QueryInputModel<CategoryFilterInputModel> queryInputModel, CancellationToken cancellationToken)
        {
            var data = await _categoryManager.GetAllAsync(queryInputModel, cancellationToken);
            return OkData(CategoryViewModel.MapAll(data.Data), new DataMetaViewModel(data.TotalCount));
        }

        [HttpGet]
        [Route("questioncategories/{id}")]
        public async Task<ActionResult<ApiResultViewModel<CategoryViewModel>>> GetByIdAsync(string id,
            CancellationToken cancellationToken)
        {
            var data = await _categoryManager.GetByIdAsyncThrowNotFoundIfNotExists(id, cancellationToken);
            return OkData(data);
        }

        [HttpPost]
        [Route("questioncategories")]
        public async Task<ActionResult<ApiResultViewModel<CategoryViewModel>>> CreateAsync(
            [FromBody]CategoryInputModel inputModel, CancellationToken cancellationToken)
        {
            var item = new Category
            {
                Title = inputModel.Title,
                Description = inputModel.Description,
                StatusId = (CategoryStatusIds)inputModel.StatusId.ToInt(),
                IconFileId = inputModel.IconFileId,
                CoverImageFileId = inputModel.CoverImageFileId,
            };

            item = await _categoryManager.SaveAsync(item, cancellationToken);

            return OkData(item);
        }

        [HttpPut]
        [Route("questioncategories/{id}")]
        public async Task<ActionResult<ApiResultViewModel<CategoryViewModel>>> UpdateAsync([FromRoute] string id,
            [FromBody]CategoryInputModel inputModel, CancellationToken cancellationToken)
        {
            var item = await _categoryManager.GetByIdAsyncThrowNotFoundIfNotExists(id, cancellationToken);

            item.Title = inputModel.Title;
            item.Description = inputModel.Description;
            item.StatusId = (CategoryStatusIds)inputModel.StatusId.ToInt();
            item.IconFileId = inputModel.IconFileId;
            item.CoverImageFileId = inputModel.CoverImageFileId;

            item = await _categoryManager.SaveAsync(item, cancellationToken);

            return OkData(item);
        }

        [HttpDelete]
        [Route("questioncategories/{id}")]
        public async Task<ActionResult> DeleteAsync([FromRoute] string id,
            CancellationToken cancellationToken)
        {
            var item = await _categoryManager.GetByIdAsyncThrowNotFoundIfNotExists(id, cancellationToken);
            await _categoryManager.DeleteAsync(item, cancellationToken);

            return OkData(item);
        }
    }
}