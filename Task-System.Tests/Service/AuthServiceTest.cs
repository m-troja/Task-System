using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_System.Data;
using Task_System.Exception.Tokens;
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
using Task_System.Model.Request;
using Task_System.Security;
using Task_System.Service;
using Task_System.Service.Impl;
using Xunit;

namespace Task_System.Tests.Service;
public class AuthServiceTest
{
    private PostgresqlDbContext GetDb()
    {
        var options = new DbContextOptionsBuilder<PostgresqlDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new PostgresqlDbContext(options);
    }
    private ILogger<AuthService> Log()
    {
        return new LoggerFactory().CreateLogger<AuthService>();
    }

    private AuthService CreateService(
        PostgresqlDbContext db,
        Mock<IUserService> mu,
        Mock<IJwtGenerator> mjwt)
    {
        return new AuthService(db, Log(), mjwt.Object, mu.Object);
    }

    [Fact]
    public void GetAccessTokenByUserId_ShouldReturnAccessToken()
    {
        // given
        int userId = 1;
        var mjwt = new Mock<IJwtGenerator>();
        var mu = new Mock<IUserService>();
        var accessToken = new AccessToken("new-access-token", DateTime.UtcNow.AddMinutes(2));

        mjwt.Setup(x => x.GenerateAccessToken(userId)).Returns(accessToken);
        var service = CreateService(GetDb(), mu, mjwt);

        // when
        var token = service.GetAccessTokenByUserId(userId);

        // then
        Assert.Equal("new-access-token", token.Token);
    }

    [Fact]
    public async void ValidateRefreshTokenRequest_ShouldReturnTrue()
    {
        // given
        var db = GetDb();
        var user1 = new User("test", "U123") { Email = "a@a.com", Id = 1 };
        var user2 = new User("test2", "U1234") { Email = "b@a.com", Id = 2 };
        var mjwt = new Mock<IJwtGenerator>();
        var mu = new Mock<IUserService>();
        var service = CreateService(db, mu, mjwt);
        var req = new RefreshTokenRequest(1, "token");

        // db 
        db.Users.Add(user1);
        db.Users.Add(user2);
        await db.SaveChangesAsync();

        var tokenForUser1 = new RefreshToken(req.RefreshToken, 1, DateTime.UtcNow.AddDays(2));
        db.RefreshTokens.Add(tokenForUser1);
        await db.SaveChangesAsync();

        // when
        var result = await service.ValidateRefreshTokenRequest(req);

        // then
        Assert.True(result);
    }

    [Fact]
    public async void ValidateRefreshTokenRequest_ShouldThrowInvalidRefreshTokenException()
    {
        // given
        var db = GetDb();
        var user1 = new User("test", "U123") { Email = "a@a.com", Id = 1 };
        var user2 = new User("test2", "U1234") { Email = "b@a.com", Id = 2 };
        var mjwt = new Mock<IJwtGenerator>();
        var mu = new Mock<IUserService>();
        var service = CreateService(db, mu, mjwt);
        var req = new RefreshTokenRequest(1, "token");

        // db 
        db.Users.Add(user1);
        db.Users.Add(user2);
        await db.SaveChangesAsync();

        var tokenForUser1 = new RefreshToken(req.RefreshToken, 1, DateTime.UtcNow.AddDays(2));
        db.RefreshTokens.Add(tokenForUser1);
        await db.SaveChangesAsync();

        // test invalid user id
        await Assert.ThrowsAsync<InvalidRefreshTokenException>( () => 
            service.ValidateRefreshTokenRequest( new RefreshTokenRequest(2, "token")));
    }

    [Fact]
    public async void ValidateRefreshTokenRequest_ShouldThrowTokenRevokedException()
    {
        // given
        var db = GetDb();
        var user1 = new User("test", "U123") { Email = "a@a.com", Id = 1 };
        var user2 = new User("test2", "U1234") { Email = "b@a.com", Id = 2 };
        var mjwt = new Mock<IJwtGenerator>();
        var mu = new Mock<IUserService>();
        var service = CreateService(db, mu, mjwt);
        var req = new RefreshTokenRequest(1, "token");

        // db 
        db.Users.Add(user1);
        db.Users.Add(user2);
        await db.SaveChangesAsync();

        var tokenForUser1 = new RefreshToken(req.RefreshToken, 1, DateTime.UtcNow.AddDays(2)) { IsRevoked = true}  ;
        db.RefreshTokens.Add(tokenForUser1);
        await db.SaveChangesAsync();

        // test invalid user id
        await Assert.ThrowsAsync<TokenRevokedException>(() =>
            service.ValidateRefreshTokenRequest(req));
    }

    [Fact]
    public async void ValidateRefreshTokenRequest_ShouldThrowTokenExpiredException()
    {
        // given
        var db = GetDb();
        var user1 = new User("test", "U123") { Email = "a@a.com", Id = 1 };
        var user2 = new User("test2", "U1234") { Email = "b@a.com", Id = 2 };
        var mjwt = new Mock<IJwtGenerator>();
        var mu = new Mock<IUserService>();
        var service = CreateService(db, mu, mjwt);
        var req = new RefreshTokenRequest(1, "token");

        // db 
        db.Users.Add(user1);
        db.Users.Add(user2);
        await db.SaveChangesAsync();

        var tokenForUser1 = new RefreshToken(req.RefreshToken, 1, DateTime.Parse("2025-10-10"));
        db.RefreshTokens.Add(tokenForUser1);
        await db.SaveChangesAsync();

        // test invalid user id
        await Assert.ThrowsAsync<TokenExpiredException>(() =>
            service.ValidateRefreshTokenRequest(req));
    }
}
