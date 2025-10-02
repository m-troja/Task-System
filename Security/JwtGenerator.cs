using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Task_System.Model.Entity;
using Task_System.Log;

namespace Task_System.Security;

public class JwtGenerator
{
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly ILogger<JwtGenerator> l;

    public JwtGenerator(IConfiguration config, ILogger<JwtGenerator> logger)
    {
        _jwtSecret = config["Jwt:Secret"] ?? throw new InvalidOperationException("JWT secret not configured.");
        _jwtIssuer = config["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT issuer not configured.");
        _jwtAudience = config["Jwt:Audience"] ?? throw new InvalidOperationException("JWT audience not configured.");
        l = logger;
    }

    public string GenerateAccessToken(int userId)
    {
        l.log($"Generating access token for userId {userId}");
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSecret);

        var claims = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        });

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddMinutes(5),
            Issuer = _jwtIssuer,
            Audience = _jwtAudience,
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
            l.log($"Refresh token: {RefreshToken.Token}, expires: {RefreshToken.Expires}");
            return RefreshToken; 
            
        }
    }
}
