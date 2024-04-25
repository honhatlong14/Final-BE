using Common.Utility;
using Data.Entities.User;

namespace Service.Utility;

public class Encryption 
{
    public static void EncryptPassword<T>(T user, string password) where T : User
    {
        if (string.IsNullOrWhiteSpace(password))
            return;
            
        user.Salt = PasswordHash.GetSalt();
        user.PasswordHash = PasswordHash.GetHash(string.Concat(password, user.Salt));
    }

    public static string DecryptPassword<T>(T user, string password) where T : User
    {
        if (string.IsNullOrWhiteSpace(password))
            return string.Empty;
            
        return PasswordHash.GetHash(string.Concat(password, user.Salt));
    }
}