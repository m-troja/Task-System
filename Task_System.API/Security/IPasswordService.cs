using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Task_System.Security;

public interface IPasswordService
{
    public string HashPassword(string password, byte[] salt);

    public byte[] GenerateSalt();

}
