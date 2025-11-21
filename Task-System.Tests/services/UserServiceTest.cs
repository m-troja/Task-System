using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Task_System.Data;
using Task_System.Exception.UserException;
using Task_System.Model.Entity;
using Task_System.Model.DTO.Cnv;
using Task_System.Service;
using Task_System.Service.Impl;
using Xunit;

namespace Task_System.Tests.Services;

public class UserServiceTests
{
    private PostgresqlDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<PostgresqlDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new PostgresqlDbContext(options);
    }

    private ILogger<UserService> GetLoggerStub()
        => new LoggerFactory().CreateLogger<UserService>();

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUser_WhenExists()
    {
        var db = GetInMemoryDb();
        var logger = GetLoggerStub();
        var chatGpt = new Mock<IChatGptService>();
        var cnv = new UserCnv();

        var user = new User("test", "U123") { Email = "a@a.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new UserService(db, logger, cnv, chatGpt.Object);

        var result = await service.GetByIdAsync(user.Id);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrow_WhenNotFound()
    {
        var db = GetInMemoryDb();
        var logger = GetLoggerStub();
        var chatGpt = new Mock<IChatGptService>();
        var cnv = new UserCnv();

        var service = new UserService(db, logger, cnv, chatGpt.Object);

        await Assert.ThrowsAsync<UserNotFoundException>(() => service.GetByIdAsync(999));
    }
    
    [Fact]
    public async Task CreateUserAsync_ShouldSaveUser()
    {
        var db = GetInMemoryDb();
        var logger = GetLoggerStub();
        var chatGpt = new Mock<IChatGptService>();
        var cnv = new UserCnv();

        var service = new UserService(db, logger, cnv, chatGpt.Object);
        var user = new User("test", "U123") { Email = "a@a.com" };

        var result = await service.CreateUserAsync(user);

        Assert.Equal(1, db.Users.Count());
        Assert.Equal(user.Email, result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenExists()
    {
        var db = GetInMemoryDb();
        var logger = GetLoggerStub();
        var chatGpt = new Mock<IChatGptService>();
        var cnv = new UserCnv();

        var user = new User("test", "U123") { Email = "a@a.com" };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new UserService(db, logger, cnv, chatGpt.Object);

        var result = await service.GetByEmailAsync("a@a.com");

        Assert.NotNull(result);
        Assert.Equal("a@a.com", result.Email);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenNotFound()
    {
        var db = GetInMemoryDb();
        var logger = GetLoggerStub();
        var chatGpt = new Mock<IChatGptService>();
        var cnv = new UserCnv();

        var service = new UserService(db, logger, cnv, chatGpt.Object);

        var result = await service.GetByEmailAsync("missing@mail.com");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetIdBySlackUserId_ShouldReturnLocalUser_WhenExists()
    {
        var db = GetInMemoryDb();
        var logger = GetLoggerStub();
        var chatGpt = new Mock<IChatGptService>();
        var cnv = new UserCnv();

        var user = new User("test", "U123");
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var service = new UserService(db, logger, cnv, chatGpt.Object);

        int id = await service.GetIdBySlackUserId("U123");

        Assert.Equal(user.Id, id);
    }


    [Fact]
    public async Task GetIdBySlackUserId_ShouldCallChatGpt_WhenNotFoundLocally()
    {
        var db = GetInMemoryDb();
        var logger = GetLoggerStub();
        var chatGpt = new Mock<IChatGptService>();
        var cnv = new UserCnv();

        // chatgpt zwraca usera
        chatGpt.Setup(x => x.GetAllChatGptUsersAsync())
               .ReturnsAsync(new List<User>
               {
                   new User("remoteUser", "U999") { Id = 77 }
               });

        // bot user
        var bot = new User("bot", "USLACKBOT") { Id = 1234 };
        db.Users.Add(bot);
        await db.SaveChangesAsync();

        var service = new UserService(db, logger, cnv, chatGpt.Object);

        int id = await service.GetIdBySlackUserId("U999");

        Assert.Equal(77, id);
    }


    [Fact]
    public async Task GetIdBySlackUserId_ShouldReturnBot_WhenUserNotFoundAnywhere()
    {
        var db = GetInMemoryDb();
        var logger = GetLoggerStub();
        var chatGpt = new Mock<IChatGptService>();
        var cnv = new UserCnv();

        chatGpt.Setup(x => x.GetAllChatGptUsersAsync())
               .ReturnsAsync(new List<User>());

        var bot = new User("bot", "USLACKBOT") { Id = 555 };
        db.Users.Add(bot);
        await db.SaveChangesAsync();

        var service = new UserService(db, logger, cnv, chatGpt.Object);

        int id = await service.GetIdBySlackUserId("UnknownSlackUser");

        Assert.Equal(555, id);
    }
}
