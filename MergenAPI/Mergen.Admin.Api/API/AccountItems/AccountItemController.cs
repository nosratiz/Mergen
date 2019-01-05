using System.Threading;
using System.Threading.Tasks;
using Mergen.Admin.Api.ViewModels;
using Mergen.Core.Managers;
using Mergen.Core.QueryProcessing;
using Microsoft.AspNetCore.Mvc;

namespace Mergen.Admin.Api.API.AccountItems
{
    public class AccountItemController : ApiControllerBase
    {
        private readonly AccountItemManager _accountItemManager;

        public AccountItemController(AccountItemManager accountItemManager)
        {
            _accountItemManager = accountItemManager;
        }

        [HttpGet("accountitems")]
        public async Task<ActionResult<ApiResultViewModel<AccountItemViewModel>>> GetItems(QueryInputModel<AccountItemFilterInputModel> queryModel, CancellationToken cancellationToken)
        {
            var data = await _accountItemManager.GetAllAsync(queryModel, cancellationToken);
            return OkData(data.Data, data.TotalCount);
        }
    }
}