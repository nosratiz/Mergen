using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Mergen.Core.Managers;
using Mergen.Core.QueryProcessing;
using Mergen.Game.Api.Helpers;
using Mergen.Game.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mergen.Game.Api.API.Shop
{
    public class ShopController : ApiControllerBase
    {
        private readonly ShopItemManager _shopItemManager;
        private readonly DataContext _dataContext;

        public ShopController(ShopItemManager shopItemManager, DataContext dataContext)
        {
            _shopItemManager = shopItemManager;
            _dataContext = dataContext;
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

        [HttpPost]
        [Route("paymentresults/{paymentUniqueId}/result")]
        public async Task<ActionResult> AccountPaymentResult(string paymentUniqueId, int state,
            CancellationToken cancellationToken)
        {
            var uniqueId = Guid.Parse(paymentUniqueId);
            var payment = await _dataContext.Payments.Include(q => q.ShopItem).FirstOrDefaultAsync(q => q.UniqueId == uniqueId, cancellationToken);
            if (payment == null)
                return NotFound();

            //TODO: Real online payment verification
            if (state == 0)
            {
                payment.State = PaymentState.Paid;

                var accountItem = await _dataContext.AccountItems.FirstOrDefaultAsync(
                    q => q.AccountId == payment.AccountId && q.ItemTypeId == payment.ShopItem.TypeId,
                    cancellationToken);

                if (accountItem != null)
                {
                    accountItem.Quantity += payment.ShopItem.Quantity ?? 1;
                }
                else
                {
                    accountItem = new AccountItem
                    {
                        AccountId = payment.AccountId,
                        ShopItemId = payment.ShopItemId,
                        Quantity = payment.ShopItem.Quantity ?? 1,
                        ItemTypeId = payment.ShopItem.TypeId
                    };

                    _dataContext.AccountItems.Add(accountItem);
                }
            }
            else
            {
                payment.State = PaymentState.Failed;
            }

            await _dataContext.SaveChangesAsync(cancellationToken);

            return Redirect($"{payment.RedirectUrl}?isSuccess={(payment.State == PaymentState.Paid ? "true" : "false")}");
        }

        [HttpPost]
        [Route("accounts/{accountId}/payments")]
        public async Task<ActionResult<ApiResultViewModel<PaymentViewModel>>> PurchaseItemByOnlinePayment(long accountId, [FromBody] PurchaseByOnlinePaymentInputModel inputModel,
            CancellationToken cancellationToken)
        {
            var account = await _dataContext.Accounts.FirstOrDefaultAsync(q => q.Id == accountId, cancellationToken);
            if (account == null)
                return NotFound();

            var shopItem = await _dataContext.ShopItems.FirstOrDefaultAsync(q => q.StatusId == ShopItemStatusIds.Available && q.Id == inputModel.ShopItemId, cancellationToken);
            if (shopItem == null)
                return BadRequest("invalid_shopItemId");

            if (shopItem.PriceTypeId != PriceTypeIds.Cash)
                return BadRequest("invalid_shopItemPriceTypeId");

            var payment = new Payment
            {
                UniqueId = Guid.NewGuid(),
                AccountId = accountId,
                Amount = shopItem.Price * shopItem.Quantity.Value,
                ShopItemId = shopItem.Id,
                CreationDateTime = DateTime.UtcNow,
                State = PaymentState.Created,
                RedirectUrl = inputModel.RedirectUrl
            };

            //TODO: BANK

            return OkData(new PaymentViewModel
            {
                CallbackUrl = Url.Action("AccountPaymentResult", new
                {
                    accountId = account.Id,
                    paymentUniqueId = payment.UniqueId.ToString("N"),
                }),
                PaygateUrl = "https://some.bank"
            });
        }

        [HttpPost]
        [Route("accounts/{accountId}/items")]
        public async Task<ActionResult> PurchaseItemByCoin(long accountId, [FromBody]PurchaseByCoinInputModel inputModel, CancellationToken cancellationToken)
        {
            var account = await _dataContext.Accounts.FirstOrDefaultAsync(q => q.Id == accountId, cancellationToken);
            if (account == null)
                return NotFound();

            var shopItem = await _dataContext.ShopItems.FirstOrDefaultAsync(q => q.StatusId == ShopItemStatusIds.Available && q.Id == inputModel.ShopItemId, cancellationToken);
            if (shopItem == null)
                return BadRequest("invalid_shopItemId");

            if (shopItem.PriceTypeId != PriceTypeIds.Coins)
                return BadRequest("invalid_shopItemPriceTypeId");

            var accountCoin = await _dataContext.AccountItems.FirstOrDefaultAsync(q =>
                q.AccountId == accountId && q.ItemTypeId == ShopItemTypeIds.Coin, cancellationToken);

            if (accountCoin == null || accountCoin.Quantity < shopItem.Price)
                return BadRequest("insufficient_funds",
                    $"You must have {shopItem.Price} coins to buy this item.");

            accountCoin.Quantity -= (int)shopItem.Price;

            var accountItem = await _dataContext.AccountItems.FirstOrDefaultAsync(q => q.AccountId == accountId && q.ItemTypeId == shopItem.TypeId, cancellationToken);

            if (accountItem != null)
            {
                accountItem.Quantity += shopItem.Quantity ?? 1;
            }
            else
            {
                accountItem = new AccountItem
                {
                    AccountId = accountId,
                    ShopItemId = shopItem.Id,
                    Quantity = shopItem.Quantity ?? 1,
                    ItemTypeId = shopItem.TypeId
                };

                _dataContext.AccountItems.Add(accountItem);
            }

            await _dataContext.SaveChangesAsync(cancellationToken);

            return Ok();
        }
    }
}
