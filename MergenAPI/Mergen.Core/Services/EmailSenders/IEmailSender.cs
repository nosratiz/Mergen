using System.Threading;
using System.Threading.Tasks;

namespace Mergen.Core.Services.EmailSenders
{
    public interface IEmailSender
    {
        Task SendSimpleEmailAsync(string from, string to, string subject, string body,
            CancellationToken cancellationToken);
    }
}