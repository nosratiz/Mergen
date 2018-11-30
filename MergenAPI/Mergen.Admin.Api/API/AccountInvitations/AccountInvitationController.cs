using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Admin.Api.ViewModels;
using Mergen.Core.Managers;
using Mergen.Core.QueryProcessing;
using Microsoft.AspNetCore.Mvc;

namespace Mergen.Admin.Api.API.AccountInvitations
{
    [ApiController]
    public class AccountInvitationController : ApiControllerBase
    {
        private readonly AccountInvitationManager _accountInvitationManager;

        public AccountInvitationController(AccountInvitationManager accountInvitationManager)
        {
            _accountInvitationManager = accountInvitationManager;
        }

        [HttpGet]
        [Route("invitations")]
        public async Task<ActionResult<ApiResultViewModel<IEnumerable<AccountInvitationViewModel>>>> GetAll(
            [FromQuery] QueryInputModel<AccountInvitationFilterInputModel> query, CancellationToken cancellationToken)
        {
            var data = await _accountInvitationManager.GetAllAsync(query, cancellationToken);
            return OkData(AccountInvitationViewModel.MapAll(data.Data), new DataMetaViewModel(data.TotalCount));
        }
    }
}