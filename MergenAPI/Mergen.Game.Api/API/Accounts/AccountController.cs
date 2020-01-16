using LazZiya.ImageResize;
using Mergen.Api.Core.Helpers;
using Mergen.Api.Core.Security.AuthenticationSystem;
using Mergen.Api.Core.ViewModels;
using Mergen.Core.Data;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Mergen.Core.Managers;
using Mergen.Core.Options;
using Mergen.Core.QueryProcessing;
using Mergen.Core.Security;
using Mergen.Core.Services;
using Mergen.Game.Api.API.Accounts.InputModels;
using Mergen.Game.Api.API.Accounts.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Mergen.Game.Api.API.Shop.InputModels;
using FileOptions = Mergen.Core.Options.FileOptions;

namespace Mergen.Game.Api.API.Accounts
{
    public class AccountController : ApiControllerBase
    {
        private readonly AccountManager _accountManager;
        private readonly IFileService _fileService;
        private readonly StatsManager _statsManager;
        private readonly AccountFriendManager _accountFriendManager;
        private readonly FriendRequestManager _friendRequestManager;
        private readonly DataContext _dataContext;
        private readonly SessionManager _sessionManager;
        private readonly AccountItemManager _accountItemManager;
        private readonly ShopItemManager _shopItemManager;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly FileManager _fileManager;
        private readonly FileOptions _options;
        private readonly JwtTokenGenerator _tokenGenerator;
        private readonly NotificationManager _notificationManager;

        public AccountController(AccountManager accountManager,
            IFileService fileService,
            StatsManager statsManager,
            AccountFriendManager accountFriendManager,
            FriendRequestManager friendRequestManager,
            DataContext dataContext,
            SessionManager sessionManager,
            AccountItemManager accountItemManager,
            ShopItemManager shopItemManager,
            IImageProcessingService imageProcessingService,
            FileManager fileManager,
            IOptions<FileOptions> options, JwtTokenGenerator tokenGenerator, NotificationManager notificationManager)
        {
            _accountManager = accountManager;
            _fileService = fileService;
            _statsManager = statsManager;
            _accountFriendManager = accountFriendManager;
            _friendRequestManager = friendRequestManager;
            _dataContext = dataContext;
            _sessionManager = sessionManager;
            _accountItemManager = accountItemManager;
            _shopItemManager = shopItemManager;
            _imageProcessingService = imageProcessingService;
            _fileManager = fileManager;
            _tokenGenerator = tokenGenerator;
            _notificationManager = notificationManager;
            _options = options.Value;
        }

        [HttpPost]
        [Route("accounts")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResultViewModel<AccountViewModel>>> Register(
            [FromBody] RegisterInputModel inputModel, CancellationToken cancellationToken)
        {
            var account = await _accountManager.FindByEmailAsync(inputModel.Email, cancellationToken);
            if (account != null)
                return BadRequest("invalid_email", "Email already exists");

            account = new Account
            {
                Email = inputModel.Email,
                PasswordHash = PasswordHash.CreateHash(inputModel.Password),
                StatusId = AccountStatusIds.Active,
                Timezone = "Asia/Tehran",
                ReceiveNotifications = true,
                SearchableByEmailAddressOrUsername = true,
                FriendsOnlyBattleInvitations = false
            };
            account.Nickname = account.Email.Substring(0, account.Email.IndexOf('@'));
            account.RegisterDateTime = DateTime.UtcNow;
            account.GenderId = GenderIds.Male;
            account = await _accountManager.SaveAsync(account, cancellationToken);

            var accountStats = new AccountStatsSummary
            {
                AccountId = account.Id,
                Level = 1
            };
            await _statsManager.SaveAsync(accountStats, cancellationToken);

            await SetDefaultAvatar(account, cancellationToken);
            await _dataContext.SaveChangesAsync(cancellationToken);

            var token = _tokenGenerator.GenerateToken(TimeSpan.FromDays(365),
                new Claim(JwtRegisteredClaimNames.Jti, account.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, account.Email),
                new Claim("Timezone", account.Timezone));

            var session = new Session
            {
                AccessToken = token,
                AccountId = account.Id,
                CreationDateTime = DateTime.UtcNow,
                StateId = SessionStateIds.Created,
                SourceAppId = AppIds.Game
            };

            await _sessionManager.SaveAsync(session, cancellationToken);

            return CreatedData(RegisterViewModel.GetRegisterViewModel(AccountViewModel.Map(account),
                SessionViewModel.Map(session)));
        }

        private async Task SetDefaultAvatar(Account account, CancellationToken cancellationToken)
        {
            var avatarTypeId = account.GenderId == GenderIds.Male ? AvatarTypeIds.Male : AvatarTypeIds.Female;
            var defaultAvatarItems = await _dataContext.ShopItems.Where(q =>
                    q.IsArchived == false &&
                    q.TypeId == ShopItemTypeIds.AvatarItem &&
                    q.DefaultAvatar == true &&
                    q.AvatarTypeId == avatarTypeId)
                .GroupBy(q => q.AvatarCategoryId).Select(q => q.First())
                .OrderBy(q => q.AvatarCategoryId)
                .ToListAsync(cancellationToken);

            if (defaultAvatarItems.Any())
            {
                var imagesToCombine = new List<Stream>();
                foreach (var item in defaultAvatarItems)
                {
                    imagesToCombine.Add(_fileService.GetFile(item.ImageFileId));
                    _dataContext.AccountItems.Add(new AccountItem
                    {
                        AccountId = account.Id,
                        ShopItemId = item.Id,
                        ItemTypeId = item.TypeId,
                        Quantity = 1,
                    });
                }

                using (var avatarImg = _imageProcessingService.Combine(imagesToCombine))
                {
                    var fileId = await _fileService.SaveFileAsync(avatarImg, cancellationToken);
                    var file = await _fileManager.SaveAsync(new UploadedFile
                    {
                        FileId = fileId,
                        CreatorAccountId = AccountId,
                        Extension = "png",
                        MimeType = "image/png",
                        MimeTypeCategoryId = UploadedFileMimeTypeCategoryIds.Image,
                        Name = "avatar",
                        Size = avatarImg.Length,
                        TypeId = UploadedFileTypeIds.AccountAvatarImage
                    }, cancellationToken);
                    account.AvatarImageId = file.FileId;
                    var uploadedImage = Image.FromStream(avatarImg);

                    var img = ImageResize.Crop(uploadedImage, 300, 250, TargetSpot.TopMiddle);

                    var filePath = Path.Combine(_options.BaseStoragePath, $"{fileId}-h.png");

                    img.SaveAs($"{filePath}");
                }

                List<Avatar> avatars = new List<Avatar>();
                foreach (var item in defaultAvatarItems)
                    avatars.Add(new Avatar { Id = item.Id, AvatarCategoryId = item.AvatarCategoryId.Value });


                avatars.Add(new Avatar { Id = 0, AvatarCategoryId = 500 });
                avatars.Add(new Avatar { Id = 0, AvatarCategoryId = 800 });
                avatars.Add(new Avatar { Id = 0, AvatarCategoryId = 900 });
                avatars.Add(new Avatar { Id = 0, AvatarCategoryId = 1200 });
                avatars.Add(new Avatar { Id = 0, AvatarCategoryId = 1300 });
                avatars.Add(new Avatar { Id = 0, AvatarCategoryId = 1400 });


                account.AvatarItemIds = JsonConvert.SerializeObject(avatars);
                await _accountManager.SaveAsync(account, cancellationToken);
            }
        }


        [HttpPut]
        [Route("accounts/{accountId}/avatar")]
        public async Task<ActionResult> SetAvatarByAccountId([FromRoute] long accountId,
            [FromBody] SetAvatarInputModel input, CancellationToken cancellationToken)
        {
            if (AccountId != accountId)
                return Forbidden();

            var account = await _accountManager.GetAsync(accountId, cancellationToken);

            var selectedAvatarItemIds = input.AvatarItemIds;
            List<Avatar> avatars = new List<Avatar>();
            if (selectedAvatarItemIds.Any())
            {
                var accountItems = await _accountItemManager.GetByAccountIdAsync(account.Id, cancellationToken);
                var imagesToCombine = new List<Stream>();

                foreach (var selectedAvatarItemId in selectedAvatarItemIds)
                {
                    var shopItem = await _shopItemManager.GetAsync(selectedAvatarItemId.Id, cancellationToken);
                    if (shopItem != null)
                    {
                        avatars.Add(selectedAvatarItemId);

                        imagesToCombine.Add(_fileService.GetFile(shopItem.ImageFileId));

                        if (!accountItems.Any(q => q.ShopItemId == selectedAvatarItemId.Id))
                        {
                            if (shopItem.DefaultAvatar == true)
                            {
                                // add item to user's items
                                var newAccountItem = new AccountItem
                                {
                                    AccountId = account.Id,
                                    ShopItemId = selectedAvatarItemId.Id,
                                    ItemTypeId = shopItem.TypeId,
                                    Quantity = 1
                                };
                                await _accountItemManager.SaveAsync(newAccountItem, cancellationToken);
                            }
                            else
                                return BadRequest("invalid_itemId", "AvatarItem not in AccountItems");

                        }
                    }
                }

                using (var avatarImg = _imageProcessingService.Combine(imagesToCombine))
                {
                    var fileId = await _fileService.SaveFileAsync(avatarImg, cancellationToken);
                    var file = await _fileManager.SaveAsync(new UploadedFile
                    {
                        FileId = fileId,
                        CreatorAccountId = AccountId,
                        Extension = "png",
                        MimeType = "image/png",
                        MimeTypeCategoryId = UploadedFileMimeTypeCategoryIds.Image,
                        Name = "avatar",
                        Size = avatarImg.Length,
                        TypeId = UploadedFileTypeIds.AccountAvatarImage
                    }, cancellationToken);

                    var uploadedImage = Image.FromStream(avatarImg);

                    var img = ImageResize.Crop(uploadedImage, 300, 250, TargetSpot.TopMiddle);

                    var filePath = Path.Combine(_options.BaseStoragePath, $"{fileId}-h.png");

                    img.SaveAs($"{filePath}");

                    account.AvatarImageId = file.FileId;
                }
            }

            account.AvatarItemIds = JsonConvert.SerializeObject(selectedAvatarItemIds);
            await _accountManager.SaveAsync(account, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("accounts/{accountId}/avatar")]
        public async Task<ActionResult> GetAvatarByAccountId([FromRoute] long accountId, [FromQuery] bool head,
            CancellationToken cancellationToken)
        {
            var account = await _accountManager.GetAsync(accountId, cancellationToken);

            if (account.AvatarImageId == null)
                return NotFound("no_avatar");

            if (head)
                return File(_fileService.GetFile($"{account.AvatarImageId}-h.png"), "image/png");

            return File(_fileService.GetFile(account.AvatarImageId), "image/png");
        }

        [HttpGet]
        [Route("accounts/{accountId}/profile")]
        public async Task<ActionResult<ApiResultViewModel<ProfileViewModel>>> GetPublicProfileByAccountId(
            [FromRoute] int accountId,
            CancellationToken cancellationToken)
        {
            var account = await _accountManager.GetAsync(accountId, cancellationToken);

            if (account == null)
                return NotFound();

            var stats = await _statsManager.GetByAccountIdAsync(accountId, cancellationToken);

            var accountStats = await _statsManager.GetByAccountIdAsync(accountId, cancellationToken);

            accountStats.Rank = await _statsManager.GetRank(accountStats.Score);

            return OkData(ProfileViewModel.Map(account, stats));
        }

        [HttpGet]
        [Route("accounts/{accountId}/stats")]
        public async Task<ActionResult<ApiResultViewModel<AccountStatsSummary>>> GetStatsByAccountId(
            [FromRoute] long accountId,
            CancellationToken cancellationToken)
        {
            var accountStats = await _statsManager.GetByAccountIdAsync(accountId, cancellationToken);

            if (accountStats == null)
                return OkData(new AccountStatsSummaryViewModel
                {
                    AccountId = accountId
                });

            accountStats.Rank = await _statsManager.GetRank(accountStats.Score);

            var accountCategoryStats = await _dataContext.AccountCategoryStats.AsNoTracking()
                .Include(q => q.Category).Where(q => q.AccountId == accountId)
                .GroupBy(x => new { x.CategoryId, x.Category }).Take(5)
                .ToListAsync(cancellationToken);

            var accountCategoryStatViewModels = accountCategoryStats.Select(x => new AccountCategoryStatViewModel
            {
                CategoryId = x.Key.CategoryId,
                CategoryTitle = x.Key.Category.Title,
                CorrectAnswersCount = x.Sum(a => a.CorrectAnswersCount),
                TotalQuestionsCount = x.Sum(a => a.TotalQuestionsCount)
            });

            return OkData(AccountStatsSummaryViewModel.Map(accountStats, accountCategoryStatViewModels.ToList()));
        }

        [HttpGet]
        [Route("accounts/profiles")]
        public async Task<ActionResult<ApiResultViewModel<IEnumerable<ProfileViewModel>>>> SearchAccounts(
            [FromQuery] string term, [FromQuery] string accountIds, [FromQuery] int page = 1, int pageSize = 30,
            CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrWhiteSpace(term) && !string.IsNullOrWhiteSpace(accountIds))
                return BadRequest("invalid_input", "cannot use term & accountIds in same query");

            var accountIdsArr = new long[0];
            if (!string.IsNullOrWhiteSpace(accountIds))
                accountIdsArr = accountIds.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(q => long.Parse(q))
                    .ToArray();

            var accounts = await _accountManager.SearchAsync(term, AccountId, accountIdsArr, page, pageSize, cancellationToken);
            return OkData(ProfileViewModel.Map(accounts));
        }



        [HttpGet]
        [Route("accounts/{accountId}/friends")]
        public async Task<ActionResult<ApiResultViewModel<IEnumerable<ProfileViewModel>>>> GetFriendsByAccountId(
            [FromRoute] long accountId, CancellationToken cancellationToken)
        {
            var friends = await _accountFriendManager.GetFriendsAsync(accountId, cancellationToken);

            List<ProfileViewModel> profileViewModels = new List<ProfileViewModel>();

            foreach (var people in friends)
            {
                people.stats.Rank = await _statsManager.GetRank(people.stats.Score);
                profileViewModels.Add(ProfileViewModel.Map(people.account, people.stats));
            }

            return OkData(profileViewModels);
        }


        [HttpDelete]
        [Route("accounts/{accountId}/friends/{friendAccountId}")]
        public async Task<ActionResult> DeleteFriendshipAsync([FromRoute] long accountId,
            [FromRoute] long friendAccountId, CancellationToken cancellationToken)
        {
            if (accountId != AccountId)
                return Forbidden();

            await _accountFriendManager.DeleteFriendshipAsync(accountId, friendAccountId, cancellationToken);

            return Ok();
        }


        [HttpPost]
        [Route("accounts/{accountId}/friendrequests")]
        public async Task<ActionResult<ApiResultViewModel<FriendRequest>>> SendFriendRequest([FromRoute] long accountId,
            [FromBody] FriendRequestInputModel inputModel, CancellationToken cancellationToken)
        {
            if (accountId != AccountId)
                return Forbidden();

            if (inputModel.FriendAccountId == accountId)
                return BadRequest("invalid_friendAccountId", "you cannot friend yourself");

            var friendAccount = await _accountManager.GetAsync(inputModel.FriendAccountId, cancellationToken);

            if (friendAccount == null || friendAccount.IsArchived)
                return BadRequest("invalid_friendAcccountId", "friend account not found.");

            var friendRequest =
                await _friendRequestManager.GetExistingRequest(accountId, inputModel.FriendAccountId,
                    cancellationToken);

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

            var account = await _accountManager.GetAsync(accountId, cancellationToken);
            friendRequest = await _friendRequestManager.SaveAsync(friendRequest, cancellationToken);

            await _notificationManager.SaveAsync(
                new Core.Entities.Notification
                {
                    AccountId = friendRequest.ToAccountId,
                    Body = $"friend request sent by {account.Email}",
                    NotificationTypeId = NotificationTypeIds.General,
                    Title = "Friend Request"
                }, cancellationToken);

            return CreatedData(friendRequest);
        }

        [HttpGet]
        [Route("friendrequests")]
        public async Task<ActionResult<ApiResultViewModel<FriendRequest>>> GetFriendRequests(
            [FromQuery] QueryInputModel<FriendRequestFilterInputModel> filterInputModel,
            CancellationToken cancellationToken)
        {
            var fromAccountId = filterInputModel.FilterParameters.FirstOrDefault(q =>
                q.FieldName == nameof(FriendRequestFilterInputModel.FromAccountId));

            var toAccountId = filterInputModel.FilterParameters.FirstOrDefault(q =>
                q.FieldName == nameof(FriendRequestFilterInputModel.ToAccountId));

            if ((fromAccountId == null || !string.Equals(fromAccountId.Values[0], AccountId.ToString(),
                     StringComparison.OrdinalIgnoreCase)) &&
                (toAccountId == null || !string.Equals(toAccountId.Values[0], AccountId.ToString(),
                     StringComparison.OrdinalIgnoreCase)))
                return Forbidden();

            var friendRequests = await _friendRequestManager.GetAllAsync(filterInputModel, cancellationToken);

            return OkData(friendRequests.Data, friendRequests.TotalCount);
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
        [Route("friendrequests/accepted")]
        public async Task<IActionResult> AcceptFriendRequest([FromBody] AcceptFriendRequestInputModel input,
            CancellationToken cancellationToken)
        {
            var friendRequest = await _friendRequestManager.GetAsync(input.FriendRequestId, cancellationToken);
            if (friendRequest == null)
                return NotFound();

            if (AccountId != friendRequest.ToAccountId)
                return Forbidden();

            if (friendRequest.StatusId != FriendRequestStatus.Pending)
                return BadRequest("invalid_state", "friend request in invalid state");

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                friendRequest.StatusId = FriendRequestStatus.Accepted;
                await _friendRequestManager.SaveAsync(friendRequest, cancellationToken);

                await _accountFriendManager.SaveAsync(new AccountFriend
                {
                    AccountId = friendRequest.FromAccountId,
                    FriendAccountId = friendRequest.ToAccountId
                }, cancellationToken);

                await _accountFriendManager.SaveAsync(new AccountFriend
                {
                    AccountId = friendRequest.ToAccountId,
                    FriendAccountId = friendRequest.FromAccountId
                }, cancellationToken);

                transaction.Complete();
            }

            return Ok();
        }

        [HttpGet]
        [Route("accounts/{accountId}")]
        public async Task<ActionResult<AccountViewModel>> GetAccountById([FromRoute] string accountId,
            CancellationToken cancellationToken)
        {
            var accId = int.Parse(accountId);

            var account = await _accountManager.GetAsync(accId, cancellationToken);
            if (account == null)
                return NotFound();

            return OkData(AccountViewModel.Map(account));
        }

        [HttpPut]
        [Route("accounts/{accountId}")]
        public async Task<ActionResult<AccountViewModel>> UpdateAccount([FromRoute] string accountId,
            [FromBody] AccountUpdateInputModel input, CancellationToken cancellationToken)
        {
            var accId = int.Parse(accountId);

            if (AccountId != accId)
                return Forbidden();

            var account = await _accountManager.GetAsync(accId, cancellationToken);
            if (account == null)
                return NotFound();

            if (account.Nickname != input.Nickname)
            {
                var existingAccountByUsername =
                    await _accountManager.FindByNicknameAsync(input.Nickname, cancellationToken);
                if (existingAccountByUsername != null)
                    return BadRequest("duplicate_username", "Another account exists using the same entered username.");
            }

            if (account.Email != input.Email)
            {
                var existingAccountByEmail = await _accountManager.FindByEmailAsync(input.Email, cancellationToken);
                if (existingAccountByEmail != null)
                    return BadRequest("duplicate_email", "Another account exists using the entered email address.");

                account.IsEmailVerified = false;
            }

            if (account.PhoneNumber != input.PhoneNumber)
            {
                var existingAccountByPhoneNumber =
                    await _accountManager.FindByPhoneNumber(input.PhoneNumber, cancellationToken);
                if (existingAccountByPhoneNumber != null)
                    return BadRequest("duplicate_phoneNumber",
                        "Another account exists using the entered phone number.");

                account.IsPhoneNumberVerified = false;
            }

            account.Nickname = input.Nickname;
            account.FirstName = input.FirstName;
            account.LastName = input.LastName;
            account.GenderId = input.GenderId.ToInt();
            account.BirthDate = input.BirthDate;
            account.PhoneNumber = input.PhoneNumber;
            account.Email = input.Email;
            account.ReceiveNotifications = input.ReceiveNotifications;
            account.SearchableByEmailAddressOrUsername = input.SearchableByEmailAddressOrUsername;
            account.FriendsOnlyBattleInvitations = input.FriendsOnlyBattleInvitations;

            account = await _accountManager.SaveAsync(account, cancellationToken);

            return OkData(AccountViewModel.Map(account));
        }

        [HttpGet]
        [Route("accounts/resetpasswordrequests/{token}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResultViewModel<ResetPasswordRequestViewModel>>> GetResetPasswordRequest(
            [FromRoute] string token, [FromServices] IOptions<ResetPasswordOptions> options,
            CancellationToken cancellationToken)
        {
            var existingAccount = await _accountManager.FindByResetPasswordTokenAsync(token, cancellationToken);
            if (existingAccount == null)
                return BadRequest("invalid_code", "Link is not valid.");

            if (existingAccount.ResetPasswordTokenGenerationTime?.Add(options.Value.ExpiresAfter) < DateTime.UtcNow)
                return BadRequest("expired_code", "Link is expired.");

            return OkData(new ResetPasswordRequestViewModel
            {
                AccountId = existingAccount.Id.ToString()
            });
        }

        [HttpPost]
        [Route("accounts/resetpasswordrequests")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResultViewModel<ResetPasswordRequestViewModel>>> ResetPasswordRequest(
            [FromBody] ResetPasswordRequestInputModel inputModel, [FromServices] IEmailService emailService,
            CancellationToken cancellationToken)
        {
            var existingAccount = await _accountManager.FindByEmailAsync(inputModel.Email, cancellationToken);

            if (existingAccount == null)
                return BadRequest("account_not_found", "Account not found.");

            await emailService.SendResetPasswordLink(existingAccount, cancellationToken);

            return CreatedData(new ResetPasswordRequestViewModel
            {
                AccountId = existingAccount.Id.ToString()
            });
        }

        [HttpPut]
        [Route("accounts/{account_id}/resetpassword")]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(ResetPasswordInputModel inputModel,
            CancellationToken cancellationToken)
        {
            var existingAccount = await _accountManager.GetAsync(int.Parse(inputModel.AccountId), cancellationToken);
            if (existingAccount == null)
                return BadRequest("invalid_account_id", "Account not found.");

            if (existingAccount.ResetPasswordToken != inputModel.Token)
                return BadRequest("invalid_code", "Link is not valid.");

            if (existingAccount.ResetPasswordTokenGenerationTime?.AddHours(48) < DateTime.UtcNow)
                return BadRequest("expired_code", "Link is expired.");

            existingAccount.PasswordHash = PasswordHash.CreateHash(inputModel.NewPassword);
            existingAccount.ResetPasswordToken = null;
            existingAccount.ResetPasswordTokenGenerationTime = null;
            await _accountManager.SaveAsync(existingAccount, cancellationToken);

            return Ok();
        }

        [HttpPut]
        [Route("accounts/{accountId}/password")]
        [AllowAnonymous]
        public async Task<ActionResult> ChangePasswordAsync(ChangePasswordInputModel inputModel,
            CancellationToken cancellationToken)
        {
            var account = await _accountManager.GetAsync(int.Parse(inputModel.AccountId), cancellationToken);
            if (account == null)
                return BadRequest("account_notfound", "Account not found.");

            if (!PasswordHash.ValidatePassword(inputModel.OldPassword, account.PasswordHash))
                return BadRequest("invalid_oldPassword", "Old password is invalid");

            account.PasswordHash = PasswordHash.CreateHash(inputModel.NewPassword);
            await _accountManager.SaveAsync(account, cancellationToken);

            return Ok();
        }

        [HttpPost]
        [Route("accounts/deactivated")]
        public async Task<ActionResult> DeactivateAccountAsync(DeactivateAccountInputModel inputModel,
            CancellationToken cancellationToken)
        {
            var account = await _accountManager.GetAsync(int.Parse(inputModel.AccountId), cancellationToken);
            if (account == null)
                return BadRequest("account_notfound", "Account not found.");

            if (account.Id != AccountId)
                return Forbidden();

            await _sessionManager.DeleteByAccountIdAsync(account.Id, cancellationToken);

            await _accountManager.ArchiveAsync(account, cancellationToken);
            return Ok();
        }

        [HttpGet]
        [Route("accounts/{accountId}/achievements")]
        public async Task<ActionResult<ApiResultViewModel<AchievementViewModel>>> GetAchievementsByAccountIdAsync(
            [FromRoute] string accountId, CancellationToken cancellationToken)
        {
            var accId = accountId.ToLong();

            var result = await (from achievementType in _dataContext.AchievementTypes.Where(q => q.IsArchived == false)
                                join achievement in _dataContext.Achievements.Where(q => q.AccountId == accId) on achievementType.Id
                                    equals achievement.AchievementTypeId
                                    into accountAchievement
                                from a in accountAchievement.DefaultIfEmpty()
                                select new AchievementViewModel
                                {
                                    AchievementType = AchievementTypeViewModel.Map(achievementType),
                                    IsAchieved = a != null
                                }).ToListAsync(cancellationToken);

            return OkData(result);
        }


        [HttpGet]
        [Route("accounts/{accountId}/ShopItems")]
        [ProducesResponseType(typeof(List<ShopItem>), 200)]
        public async Task<IActionResult> GetShopItems(long accountId, CancellationToken cancellationToken)
        {
            var account = await _accountManager.GetAsync(accountId, cancellationToken);
            if (account == null)
                return NotFound();

            var payments = await _dataContext.Payments.Include(x => x.ShopItem)
                .Where(x => x.IsArchived == false && x.AccountId == accountId).ToListAsync(cancellationToken);


            List<ShopItem> shopItems = new List<ShopItem>();

            foreach (var item in payments)
                shopItems.Add(item.ShopItem);


            return Ok(ShopItemViewModel.MapAll(shopItems));
        }
    }
}