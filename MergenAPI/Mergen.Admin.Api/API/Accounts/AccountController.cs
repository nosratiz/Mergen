using System;
using System.Collections.Generic;
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
using Mergen.Core.Managers;
using Mergen.Core.Options;
using Mergen.Core.QueryProcessing;
using Mergen.Core.Security;
using Mergen.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Mergen.Admin.Api.API.Accounts
{
    [ApiController]
    public class AccountController : ApiControllerBase
    {
        private readonly AccountManager _accountManager;
        private readonly IEmailService _emailService;
        private readonly EmailVerificationOptions _emailVerificationOptions;
        private readonly ResetPasswordOptions _resetPasswordOptions;

        public AccountController(AccountManager accountManager, IEmailService emailService,
            FileManager fileManager,
            IOptions<EmailVerificationOptions> emailVerificationOptions,
            IOptions<ResetPasswordOptions> resetPasswordOptions,
            IOptions<FinancialOptions> financialOptions,
            IFileService fileService,
            IHostingEnvironment hostingEnvironment,
            IOptions<FileOptions> fileOptions,
            JwtTokenGenerator tokenGenerator,
            SessionManager sessionManager)
        {
            _accountManager = accountManager;
            _emailService = emailService;
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
        public async Task<ActionResult<ApiResultViewModel<AccountViewModel>>> UpdateAccount(string accountId,
            [FromBody] AccountInputModel inputModel, CancellationToken cancellationToken)
        {
            var account = await _accountManager.GetAsync(accountId.ToInt(), cancellationToken);
            if (account is null)
                return NotFound();

            if (await _accountManager.FindByEmailAsync(account.Email, cancellationToken) != null)
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

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
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