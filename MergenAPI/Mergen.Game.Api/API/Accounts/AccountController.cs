using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Mergen.Core.Managers;
using Mergen.Core.QueryProcessing;
using Mergen.Core.Security;
using Mergen.Core.Services;
using Mergen.Game.Api.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Mergen.Game.Api.API.Accounts
{
    public class AccountController : ApiControllerBase
    {
        private readonly AccountManager _accountManager;
        private readonly IFileService _fileService;
        private readonly StatsManager _statsManager;
        private readonly AccountFriendManager _accountFriendManager;
        private readonly FriendRequestManager _friendRequestManager;

        public AccountController(AccountManager accountManager, IFileService fileService, StatsManager statsManager, AccountFriendManager accountFriendManager, FriendRequestManager friendRequestManager)
        {
            _accountManager = accountManager;
            _fileService = fileService;
            _statsManager = statsManager;
            _accountFriendManager = accountFriendManager;
            _friendRequestManager = friendRequestManager;
        }

        [HttpPost]
        [Route("accounts")]
        public async Task<ActionResult<ApiResultViewModel<AccountViewModel>>> Register([FromBody]RegisterInputModel inputModel, CancellationToken cancellationToken)
        {
            var account = await _accountManager.FindByEmailAsync(inputModel.Email, cancellationToken);
            if (account != null)
                return BadRequest("invalid_email", "Email already exists");

            account = new Account();
            account.Email = inputModel.Email;
            account.PasswordHash = PasswordHash.CreateHash(inputModel.Password);
            account.StatusId = AccountStatusIds.Active;
            account = await _accountManager.SaveAsync(account, cancellationToken);

            return CreatedData(AccountViewModel.Map(account));
        }

        [HttpGet]
        [Route("accounts/{accountId}")]
        public async Task<ActionResult<ApiResultViewModel<AccountViewModel>>> GetAccountById([FromRoute] int accountId,
            CancellationToken cancellationToken)
        {
            return OkData(AccountViewModel.Map(await _accountManager.GetAsync(accountId, cancellationToken)));
        }

        [HttpGet]
        [Route("accounts/{accountId}/avatar")]
        public async Task<ActionResult> GetAvatarByAccountId([FromRoute] long accountId,
            CancellationToken cancellationToken)
        {
            var account = await _accountManager.GetAsync(accountId, cancellationToken);
            return File(_fileService.GetFile(account.AvatarImageId.ToString()), "image/png");
        }

        [HttpGet]
        [Route("accounts/{accountId}/profile")]
        public async Task<ActionResult<ApiResultViewModel<ProfileViewModel>>> GetPublicProfileByAccountId([FromRoute] int accountId,
            CancellationToken cancellationToken)
        {
            var account = await _accountManager.GetAsync(accountId, cancellationToken);
            if (account == null)
                return NotFound();

            var stats = await _statsManager.GetByAccountIdAsync(accountId, cancellationToken);

            return OkData(ProfileViewModel.Map(account, stats));
        }

        [HttpGet]
        [Route("accounts/{accountId}/stats")]
        public async Task<ActionResult<ApiResultViewModel<AccountStatsSummary>>> GetStatsByAccountId([FromRoute] long accountId,
            CancellationToken cancellationToken)
        {
            var accountStats = await _statsManager.GetByAccountIdAsync(accountId, cancellationToken);
            return OkData(accountStats);
        }

        [HttpGet]
        [Route("accounts/profiles")]
        public async Task<ActionResult<ApiResultViewModel<IEnumerable<ProfileViewModel>>>> SearchAccounts([FromQuery] string term,
            CancellationToken cancellationToken)
        {
            var accounts = await _accountManager.SearchAsync(term, cancellationToken);
            return OkData(ProfileViewModel.Map(accounts));
        }

        [HttpGet]
        [Route("accounts/{accountId}/friends")]
        public async Task<ActionResult<ApiResultViewModel<IEnumerable<ProfileViewModel>>>> GetFriendsByAccountId([FromRoute] long accountId, CancellationToken cancellationToken)
        {
            var friends = await _accountFriendManager.GetFriendsAsync(accountId, cancellationToken);
            return OkData(ProfileViewModel.Map(friends));
        }

        [HttpPost]
        [Route("accounts/{accountId}/friendrequests")]
        public async Task<ActionResult<ApiResultViewModel<FriendRequest>>> SendFriendRequest([FromRoute] long accountId,
            [FromBody] FriendRequestInputModel inputModel, CancellationToken cancellationToken)
        {
            var friendRequest = await _friendRequestManager.GetExistingRequest(accountId, inputModel.FriendAccountId, cancellationToken);

            if (friendRequest != null)
                return BadRequest("already_exists", "Friend request already sent.");

            if (await _accountFriendManager.IsFriendAsync(accountId, inputModel.FriendAccountId, cancellationToken))
                return BadRequest("already_friends", "Requested player is already in your friend list.");

            friendRequest = new FriendRequest
            {
                FromAccountId = accountId,
                ToAccountId = inputModel.FriendAccountId,
                RequestDateTime = DateTime.UtcNow,
                StatusId = FriendRequestStatus.Pending
            };

            friendRequest = await _friendRequestManager.SaveAsync(friendRequest, cancellationToken);

            return CreatedData(friendRequest);
        }

        [HttpGet]
        [Route("friendrequests")]
        public async Task<ActionResult<ApiResultViewModel<FriendRequest>>> GetFriendRequests(
            [FromQuery] QueryInputModel<FriendRequestFilterInputModel> filterInputModel, CancellationToken cancellationToken)
        {
            return OkData(_friendRequestManager.GetAllAsync(filterInputModel, cancellationToken));
        }

        [HttpPost]
        [Route("friendrequests/ignored")]
        public async Task<IActionResult> IgnoreFriendRequest([FromBody] long friendRequestId,
            CancellationToken cancellationToken)
        {
            var friendRequest = await _friendRequestManager.GetAsync(friendRequestId, cancellationToken);
            if (friendRequest == null)
                return NotFound();

            if (AccountId != friendRequest.ToAccountId)
                return Forbidden();

            if (friendRequest.StatusId != FriendRequestStatus.Pending)
                return BadRequest("invalid_state", "friend request in invalid state");

            friendRequest.StatusId = FriendRequestStatus.Ignored;
            await _friendRequestManager.SaveAsync(friendRequest, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Route("friendrequests/cancelled")]
        public async Task<IActionResult> CancelFriendRequest([FromBody] long friendRequestId,
            CancellationToken cancellationToken)
        {
            var friendRequest = await _friendRequestManager.GetAsync(friendRequestId, cancellationToken);
            if (friendRequest == null)
                return NotFound();

            if (AccountId != friendRequest.FromAccountId)
                return Forbidden();

            if (friendRequest.StatusId != FriendRequestStatus.Pending)
                return BadRequest("invalid_state", "friend request in invalid state");

            friendRequest.StatusId = FriendRequestStatus.Cancelled;
            await _friendRequestManager.SaveAsync(friendRequest, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Route("friendrequests/cancelled")]
        public async Task<IActionResult> AcceptFriendRequest([FromBody] long friendRequestId,
            CancellationToken cancellationToken)
        {
            var friendRequest = await _friendRequestManager.GetAsync(friendRequestId, cancellationToken);
            if (friendRequest == null)
                return NotFound();

            if (AccountId != friendRequest.ToAccountId)
                return Forbidden();

            if (friendRequest.StatusId != FriendRequestStatus.Pending)
                return BadRequest("invalid_state", "friend request in invalid state");

            friendRequest.StatusId = FriendRequestStatus.Accepted;
            await _friendRequestManager.SaveAsync(friendRequest, cancellationToken);

            return Ok();
        }
    }
}