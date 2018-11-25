using System.Threading;
using System.Threading.Tasks;
using Mergen.Core.Entities;

namespace Mergen.Core.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipient, string subject, string text, CancellationToken cancellationToken);
        Task SendEmailVerificationTokenAsync(Account account, CancellationToken cancellationToken);
        Task SendResetPasswordLink(Account account, CancellationToken cancellationToken);
    }
}