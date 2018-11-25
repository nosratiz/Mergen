using System;
using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Entities;
using Mergen.Core.Managers;
using Mergen.Core.Options;
using Mergen.Core.Resources;
using Mergen.Core.Security;
using Mergen.Core.Services.EmailSenders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Mergen.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly AccountManager _accountManager;
        private readonly IStringLocalizer<Emails> _stringLocalizer;
        private readonly IEmailSender _emailSender;
        private readonly EmailVerificationOptions _emailVerificationOptions;
        private readonly ResetPasswordOptions _resetPasswordOptions;

        public EmailService(AccountManager accountManager, IStringLocalizer<Emails> stringLocalizer,
            IEmailSender emailSender,
            IOptions<EmailVerificationOptions> emailConfirmationOptions,
            IOptions<ResetPasswordOptions> resetPasswordOptions)
        {
            _accountManager = accountManager;
            _stringLocalizer = stringLocalizer;
            _emailSender = emailSender;
            _emailVerificationOptions = emailConfirmationOptions.Value;
            _resetPasswordOptions = resetPasswordOptions.Value;
        }

        public async Task SendEmailAsync(string recipient, string subject, string text,
            CancellationToken cancellationToken)
        {
//			Console.WriteLine($"SEND EMAIL, Recipient:{recipient}, Subject:{subject}, Text:{text}");
            await _emailSender.SendSimpleEmailAsync(null, recipient, subject, text, cancellationToken);
        }

        public async Task SendEmailVerificationTokenAsync(Account account, CancellationToken cancellationToken)
        {
            if (account.EmailVerificationTokenGenerationTime == null ||
                account.EmailVerificationTokenGenerationTime.Value.Add(_emailVerificationOptions.ExpiresAfter) <
                DateTime.UtcNow)
            {
                account.EmailVerificationToken = VerificationHelper.GenerateEmailToken();
                account.EmailVerificationTokenGenerationTime = DateTime.UtcNow;
            }

            await _accountManager.SaveAsync(account, cancellationToken);
            var url = string.Format(_emailVerificationOptions.LinkFormat, account.Id, account.EmailVerificationToken);
            var title = account.Email;
            await SendEmailAsync(account.Email, _stringLocalizer["Verification.Subject"],
                string.Format(_stringLocalizer["Verification.Body"], title, url), cancellationToken);
        }

        public async Task SendResetPasswordLink(Account account, CancellationToken cancellationToken)
        {
            if (account.ResetPasswordTokenGenerationTime == null ||
                account.ResetPasswordTokenGenerationTime.Value.Add(_resetPasswordOptions.ExpiresAfter) <
                DateTime.UtcNow)
            {
                account.ResetPasswordToken = VerificationHelper.GenerateEmailToken();
                account.ResetPasswordTokenGenerationTime = DateTime.UtcNow;
            }

            await _accountManager.SaveAsync(account, cancellationToken);
            var url = string.Format(_resetPasswordOptions.LinkFormat, account.Id, account.ResetPasswordToken);
            var title = account.Email;
            await SendEmailAsync(account.Email, _stringLocalizer["ResetPassword.Subject"],
                string.Format(_stringLocalizer["ResetPassword.Body"], title, url), cancellationToken);
        }
    }
}