using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Mergen.Core.Services.EmailSenders
{
    public class GmailEmailSender : IEmailSender
    {
        private readonly GmailEmailSenderOptions _options;

        public GmailEmailSender(IOptions<GmailEmailSenderOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendSimpleEmailAsync(string from, string to, string subject, string body,
            CancellationToken cancellationToken)
        {
            if (!_options.IsEnabled)
                return;

            var smtp = new SmtpClient(_options.Host, _options.Port);

            var creds = new NetworkCredential(_options.Account, _options.Password);

            smtp.UseDefaultCredentials = false;
            smtp.Credentials = creds;
            smtp.Port = _options.Port;
            smtp.EnableSsl = true;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Timeout = 20000;

            if (string.IsNullOrEmpty(from))
                from = _options.From;

            var toAddess = new MailAddress(to);
            var fromAddress = new MailAddress(from, _options.Title);

            var msg = new MailMessage();

            msg.To.Add(toAddess);
            msg.From = fromAddress;
            msg.IsBodyHtml = true;
            msg.Subject = subject;
            msg.Body = body;

            await smtp.SendMailAsync(msg);
        }
    }
}