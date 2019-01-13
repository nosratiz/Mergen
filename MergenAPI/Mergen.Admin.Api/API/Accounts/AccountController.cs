using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Mergen.Admin.Api.API.Accounts.InputModels;
using Mergen.Admin.Api.API.Accounts.ViewModels;
using Mergen.Admin.Api.Helpers;
using Mergen.Admin.Api.Security.AuthenticationSystem;
using Mergen.Admin.Api.ViewModels;
using Mergen.Core.Entities;
using Mergen.Core.EntityIds;
using Mergen.Core.Managers;
using Mergen.Core.Options;
using Mergen.Core.QueryProcessing;
using Mergen.Core.Security;
using Mergen.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using FileOptions = Mergen.Core.Options.FileOptions;

namespace Mergen.Admin.Api.API.Accounts
{
    [ApiController]
    public class AccountController : ApiControllerBase
    {
        private readonly AccountManager _accountManager;
        private readonly IEmailService _emailService;
        private readonly FileManager _fileManager;
        private readonly IFileService _fileService;
        private readonly EmailVerificationOptions _emailVerificationOptions;
        private readonly ResetPasswordOptions _resetPasswordOptions;
        private AccountItemManager _accountItemManager;
        private ShopItemManager _shopItemManager;
        private readonly IImageProcessingService _imageProcessingService;

        public AccountController(AccountManager accountManager, IEmailService emailService,
            FileManager fileManager,
            IOptions<EmailVerificationOptions> emailVerificationOptions,
            IOptions<ResetPasswordOptions> resetPasswordOptions,
            IOptions<FinancialOptions> financialOptions,
            IFileService fileService,
            IHostingEnvironment hostingEnvironment,
            IOptions<FileOptions> fileOptions,
            JwtTokenGenerator tokenGenerator,
            SessionManager sessionManager,
            AccountItemManager accountItemManager,
            ShopItemManager shopItemManager,
            IImageProcessingService imageProcessingService)
        {
            _accountManager = accountManager;
            _emailService = emailService;
            _fileManager = fileManager;
            _fileService = fileService;
            _accountItemManager = accountItemManager;
            _shopItemManager = shopItemManager;
            _imageProcessingService = imageProcessingService;
            _emailVerificationOptions = emailVerificationOptions.Value;
            _resetPasswordOptions = resetPasswordOptions.Value;
        }

        [HttpPost]
        [Route("accounts")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResultViewModel<AccountViewModel>>> CreateAccount(
            [FromBody] AccountInputModel inputModel,
            CancellationToken cancellationToken)
        {
            var account = new Account();
            account.Email = inputModel.Email;
            account.PasswordHash = PasswordHash.CreateHash(inputModel.Password);
            account.PhoneNumber = inputModel.PhoneNumber;
            account.FirstName = inputModel.FirstName;
            account.LastName = inputModel.LastName;
            account.Nickname = inputModel.Nickname;
            account.GenderId = inputModel.GenderTypeId?.ToInt();
            account.BirthDate = inputModel.BirthDate;
            account.StatusId = inputModel.StatusId.ToInt();
            account.StatusNote = inputModel.StatusNote;
            account.IsEmailVerified = inputModel.IsEmailVerified;
            account.IsPhoneNumberVerified = inputModel.IsPhoneNumberVerified;
            account.Timezone = "Asia/Tehran";

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                account = await _accountManager.SaveAsync(account, cancellationToken);
                await _accountManager.UpdateRolesAsync(account, inputModel.RoleIds.Select(rid => rid.ToLong()), cancellationToken);

                transaction.Complete();
            }

            return OkData(AccountViewModel.Map(account));
        }

        [HttpPut]
        [Route("accounts/{id}")]
        public async Task<ActionResult<ApiResultViewModel<AccountViewModel>>> UpdateAccount(string id,
            [FromBody] AccountInputModel inputModel, CancellationToken cancellationToken)
        {
            var account = await _accountManager.GetAsync(id.ToInt(), cancellationToken);
            if (account is null)
                return NotFound();

            if (account.Email != inputModel.Email && await _accountManager.FindByEmailAsync(inputModel.Email, cancellationToken) != null)
                return BadRequest("duplicate_email", "Account with entered email already exists.");

            account.Email = inputModel.Email;

            if (!string.IsNullOrWhiteSpace(inputModel.Password))
                account.PasswordHash = PasswordHash.CreateHash(inputModel.Password);

            account.PhoneNumber = inputModel.PhoneNumber;
            account.FirstName = inputModel.FirstName;
            account.LastName = inputModel.LastName;
            account.Nickname = inputModel.Nickname;
            account.GenderId = inputModel.GenderTypeId?.ToInt();
            account.BirthDate = inputModel.BirthDate;
            account.StatusId = inputModel.StatusId.ToInt();
            account.StatusNote = inputModel.StatusNote;
            account.IsEmailVerified = inputModel.IsEmailVerified;
            account.IsPhoneNumberVerified = inputModel.IsPhoneNumberVerified;
            account.Timezone = "Asia/Tehran";
            account.CoverImageId = inputModel.CoverImageId;

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var selectedAvatarItemIds = inputModel.AvatarItemIds.Select(q => q.ToLong()).ToArray();
                if (selectedAvatarItemIds.Any())
                {
                    var accountItems = await _accountItemManager.GetByAccountIdAsync(account.Id, cancellationToken);
                    var imagesToCombine = new List<Stream>();
                    foreach (var selectedAvatarItemId in selectedAvatarItemIds)
                    {
                        var shopItem = await _shopItemManager.GetAsync(selectedAvatarItemId, cancellationToken);
                        imagesToCombine.Add(_fileService.GetFile(shopItem.ImageFileId));

                        if (!accountItems.Any(q => q.ShopItemId == selectedAvatarItemId))
                        {
                            // add item to user's items
                            var newAccountItem = new AccountItem
                            {
                                AccountId = account.Id,
                                ShopItemId = selectedAvatarItemId,
                                ItemTypeId = shopItem.TypeId,
                                Quantity = 1
                            };
                            newAccountItem = await _accountItemManager.SaveAsync(newAccountItem, cancellationToken);
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
                        account.AvatarImageId = file.FileId;
                    }
                }

                account.AvatarItemIds = JsonConvert.SerializeObject(selectedAvatarItemIds);
                account.RoleIds = JsonConvert.SerializeObject(inputModel.RoleIds?.Select(q=>q.ToLong()) ?? new long[0]);
                account = await _accountManager.SaveAsync(account, cancellationToken);
                await _accountManager.UpdateRolesAsync(account, inputModel.RoleIds.Select(rid => rid.ToLong()), cancellationToken);

                transaction.Complete();
            }

            return OkData(AccountViewModel.Map(account));
        }

        [HttpGet]
        [Route("accounts/{id}")]
        public async Task<ActionResult<ApiResultViewModel<AccountViewModel>>> GetAccountById([FromRoute] string id,
            CancellationToken cancellationToken)
        {
            var account = await _accountManager.GetAsync(id.ToInt(), cancellationToken);
            if (account == null)
                return NotFound();

            return OkData(AccountViewModel.Map(account));
        }

        [HttpPut]
        [Route("accounts/{accountId}/password")]
        [AllowAnonymous]
        public async Task<ActionResult> ChangePasswordAsync(ChangePasswordInputModel inputModel,
            CancellationToken cancellationToken)
        {
            var account = await _accountManager.GetAsync(int.Parse(inputModel.AccountId), cancellationToken);
            if (account == null)
                return BadRequest("account_notfound", "حساب کاربری پیدا نشد.");

            if (!PasswordHash.ValidatePassword(inputModel.OldPassword, account.PasswordHash))
                return BadRequest("invalid_oldPassword", "کلمه عبور قبلی اشتباه است.");

            account.PasswordHash = PasswordHash.CreateHash(inputModel.NewPassword);
            await _accountManager.SaveAsync(account, cancellationToken);

            return Ok();
        }

        [HttpGet]
        [Route("accounts")]
        public async Task<ActionResult<ApiResultViewModel<IEnumerable<AccountViewModel>>>> GetAccounts(
            QueryInputModel<AccountFilterInputModel> inputModel,
            CancellationToken cancellationToken)
        {
            var queryResult = await _accountManager.GetAllAsync(inputModel, cancellationToken);
            return OkData(AccountViewModel.MapAll(queryResult.Data), new DataMetaViewModel(queryResult.TotalCount));
        }

        [HttpPut]
        [Route("accounts/{accountId}/emailverification")]
        [AllowAnonymous]
        public async Task<ActionResult> VerifyEmail(string accountId, EmailVerificationInputModel inputModel,
            CancellationToken cancellationToken)
        {
            var account = await _accountManager.GetAsync(int.Parse(accountId), cancellationToken);
            if (account == null)
                return BadRequest("account_notfound", "Account not found");

            if (account.EmailVerificationToken != inputModel.Token)
                return BadRequest("invalid_token", "Verification token is not valid.");

            if (account.EmailVerificationTokenGenerationTime?.Add(_emailVerificationOptions.ExpiresAfter) <
                DateTime.UtcNow)
                return BadRequest("invalid_token", "Verification link is expired.");

            account.IsEmailVerified = true;
            await _accountManager.SaveAsync(account, cancellationToken);
            return Ok();
        }

        [HttpGet]
        [Route("accounts/resetpasswordrequests/{token}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResultViewModel<ResetPasswordRequestViewModel>>> GetResetPasswordRequest(
            string token,
            CancellationToken cancellationToken)
        {
            var existingAccount = await _accountManager.FindByResetPasswordTokenAsync(token, cancellationToken);
            if (existingAccount == null)
                return BadRequest("invalid_code", "Link is not valid.");

            if (existingAccount.ResetPasswordTokenGenerationTime?.Add(_resetPasswordOptions.ExpiresAfter) <
                DateTime.UtcNow)
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
            ResetPasswordRequestInputModel inputModel, CancellationToken cancellationToken)
        {
            var existingAccount = await _accountManager.FindByEmailAsync(inputModel.Email, cancellationToken);
            if (existingAccount == null)
                return BadRequest("account_not_found", "Account not found.");

            await _emailService.SendResetPasswordLink(existingAccount, cancellationToken);

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
    }
}