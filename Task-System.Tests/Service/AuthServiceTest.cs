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
using Task_System.Model.DTO.Cnv;
using Task_System.Model.Entity;
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

}
