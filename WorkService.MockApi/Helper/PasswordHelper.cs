using System.Security.Cryptography;
using System.Text;

namespace WorkService.MockApi.Helpers
{
    public static class PasswordHelper
    {
        public static string GenerateSalt()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8);
        }

        public static string Md5WithSalt(string password, string salt)
        {
            using var md5 = MD5.Create();
            var input = Encoding.UTF8.GetBytes(password + salt);
            var hash = md5.ComputeHash(input);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
