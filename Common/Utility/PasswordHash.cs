using System.Security.Cryptography;
using System.Text;

namespace Common.Utility;

public class PasswordHash
{
    public static string GetHash(string text)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }

    public static string GetSalt()
    {
        byte[] bytes = new byte[128 / 8];
        using (var keyGenerator = RandomNumberGenerator.Create())
        {
            keyGenerator.GetBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}