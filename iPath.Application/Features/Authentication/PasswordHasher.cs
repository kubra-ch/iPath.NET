using System.Security.Cryptography;
using System.Text;

namespace iPath.Application.Areas.Authentication;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyHashedPassword(string hashedPassword, string providedPassword);
}

public class PasswordHasher : IPasswordHasher
{
    private static string _salt = "iPathSalt";

    private string CreateHash(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var inputHash = SHA256.HashData(inputBytes);
        return Convert.ToHexString(inputHash);
    }

    public string HashPassword(string password)
    {
        return CreateHash(_salt + password!);
    }

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        var check = CreateHash(_salt + providedPassword!);
        return hashedPassword == check;
    }
}