using System.Security.Cryptography;
using System.Text;

namespace Mergen.Core.Security
{
    public static class VerificationHelper
    {
        private static readonly char[] AvailableCharacters =
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'K', 'L', 'M', 'N', 'P', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y',
            'Z', '2',
            '3', '4', '5', '6', '7', '8', '9'
        };

        public static string GetUniqueKey(int length)
        {
            byte[] data;
            using (var crypto = new RNGCryptoServiceProvider())
            {
                data = new byte[length];
                crypto.GetNonZeroBytes(data);
            }

            var result = new StringBuilder(length);
            foreach (var b in data)
            {
                result.Append(AvailableCharacters[b % AvailableCharacters.Length]);
            }

            return result.ToString();
        }

        public static string GenerateEmailToken(int length = 128)
        {
            return GetUniqueKey(length);
        }

        public static string GenerateSmsToken(int length = 6)
        {
            return GetUniqueKey(length);
        }
    }
}