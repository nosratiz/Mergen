namespace Mergen.Core.Services.EmailSenders
{
    public class GmailEmailSenderOptions
    {
        public bool IsEnabled { get; set; }
        public int Port { get; set; }
        public string From { get; set; }
        public string Host { get; set; }
        public string Title { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
    }
}