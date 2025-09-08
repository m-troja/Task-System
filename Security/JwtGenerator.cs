using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Task_System.Model.Entity;
using Task_System.Config;

namespace Task_System.Security;

public class JwtGenerator
{
    private readonly string _jwtSecret ;
    private readonly ILogger<JwtGenerator> l;

    public JwtGenerator(string jwtSecret, ILogger<JwtGenerator> l)
    {
        _jwtSecret = jwtSecret;
        this.l = l;
    }

    public string GenerateAccessToken(int userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }),
            Expires = DateTime.UtcNow.AddMinutes(5), // access token 5 min
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        l.log($"Access token: {token}");
        return tokenHandler.WriteToken(token);
    }

    public RefreshToken GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            var RefreshToken = new RefreshToken(Convert.ToBase64String(randomNumber), DateTime.UtcNow.AddDays(7));
            l.log($"Refresh token: {RefreshToken.ToString}");
            return RefreshToken; 
            
        }
    }
}
