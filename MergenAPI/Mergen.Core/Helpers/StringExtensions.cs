namespace Mergen.Core.Helpers
{
    public static class StringExtensions
    {
        public static string NormalizeEmail(this string email)
        {
            return email.ToUpperInvariant();
        }
    }
}