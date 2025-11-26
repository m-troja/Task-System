using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Task_System.Model.Entity;
using Task_System.Log;

namespace Task_System.Security;

public interface IJwtGenerator
{
    public AccessToken GenerateAccessToken(int userId);

    public RefreshToken GenerateRefreshToken(int userId, User user);

}
