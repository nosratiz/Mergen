using System;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Api.Core.Helpers;
using Mergen.Api.Core.ViewModels;
using Mergen.Core.Entities;
using Mergen.Core.Managers;
using Mergen.Core.QueryProcessing;
using Microsoft.AspNetCore.Mvc;

namespace Mergen.Admin.Api.API.ShopItems
{
    [ApiController]
    public class ShopItemController : ApiControllerBase
    {
        private readonly ShopItemManager _shopItemManager;

        public ShopItemController(ShopItemManager shopItemManager)
        {
            _shopItemManager = shopItemManager;
        }

        [HttpPost]
        [Route("shopitems")]
        public async Task<ActionResult<ApiResultViewModel<ShopItemViewModel>>> Create([FromBody]ShopItemInputModel inputModel,
            CancellationToken cancellationToken)
        {

            var shopItem = new ShopItem
            {
                Title = inputModel.Title,
                Price = inputModel.Price,
                PriceTypeId = inputModel.PriceTypeId.ToInt(),
                TypeId = inputModel.TypeId.ToInt(),
                StatusId = inputModel.StatusId.ToInt(),
                ImageFileId = inputModel.ImageFileId,
                UnlockLevel = inputModel.UnlockLevel,
                UnlockSky = inputModel.UnlockSky,
                AvatarCategoryId = inputModel.AvatarCategoryId?.ToInt(),
                AvatarTypeId = inputModel.AvatarTypeId?.ToInt(),
                DefaultAvatar = inputModel.DefaultAvatar,
                Description = inputModel.Description
            };

            shopItem = await _shopItemManager.SaveAsync(shopItem, cancellationToken);

            return OkData(ShopItemViewModel.Map(shopItem));
        }

        [HttpPut]
        [Route("shopitems/{id}")]
        public async Task<ActionResult<ApiResultViewModel<ShopItemViewModel>>> Update([FromRoute]string id, [FromBody]ShopItemInputModel inputModel,
            CancellationToken cancellationToken)
        {
            //TODO: what if somebody has bought a shop item and we update it?

            var shopItem = await _shopItemManager.GetByIdAsyncThrowNotFoundIfNotExists(id, cancellationToken);

            shopItem.Title = inputModel.Title;
            shopItem.Price = inputModel.Price;
            shopItem.PriceTypeId = inputModel.PriceTypeId.ToInt();
            shopItem.TypeId = inputModel.TypeId.ToInt();
            shopItem.StatusId = inputModel.StatusId.ToInt();
            shopItem.ImageFileId = inputModel.ImageFileId;
            shopItem.UnlockLevel = inputModel.UnlockLevel;
            shopItem.UnlockSky = inputModel.UnlockSky;
            shopItem.AvatarCategoryId = inputModel.AvatarCategoryId?.ToInt();
            shopItem.AvatarTypeId = inputModel.AvatarTypeId?.ToInt();
            shopItem.DefaultAvatar = inputModel.DefaultAvatar;
            shopItem.Description = inputModel.Description;

            shopItem = await _shopItemManager.SaveAsync(shopItem, cancellationToken);

            return OkData(ShopItemViewModel.Map(shopItem));
        }

        [HttpGet]
        [Route("shopitems/{id}")]
        public async Task<ActionResult<ApiResultViewModel<ShopItemViewModel>>> GetById([FromRoute] string id,
            CancellationToken cancellationToken)
        {
            var shopItem = await _shopItemManager.GetByIdAsyncThrowNotFoundIfNotExists(id, cancellationToken);
            return OkData(ShopItemViewModel.Map(shopItem));
        }

        [HttpGet]
        [Route("shopitems")]
        public async Task<ActionResult<ApiResultViewModel<ShopItemViewModel>>> GetAll([FromQuery]QueryInputModel<ShopItemFilterInputModel> query,
            CancellationToken cancellationToken)
        {
            var result = await _shopItemManager.GetAllAsync(query, cancellationToken);
            return OkData(ShopItemViewModel.MapAll(result.Data), result.TotalCount);
        }

        [HttpDelete]
        [Route("shopitems/{id}")]
        public async Task<ActionResult> Delete([FromRoute] string id,
            CancellationToken cancellationToken)
        {
            //TODO: what if somebody has bought a shop item and we delete it? impossible, should update statusid to unavailable

            var shopItem = await _shopItemManager.GetByIdAsyncThrowNotFoundIfNotExists(id, cancellationToken);

            await _shopItemManager.ArchiveAsync(shopItem, cancellationToken);

            return Ok();
        }
    }
}