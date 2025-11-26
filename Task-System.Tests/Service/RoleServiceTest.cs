using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Task_System.Data;
using Task_System.Model.Entity;
using Task_System.Service.Impl;
using Xunit;

namespace Task_System.Tests.Service;

public class RoleServiceTests
{
    private PostgresqlDbContext GetInMemoryDb()
    {
        var options = new DbContextOptionsBuilder<PostgresqlDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PostgresqlDbContext(options);
    }

    [Fact]
    public async Task GetRoleByName_ShouldReturnRole_WhenExists()
    {
        // given
        await using var db = GetInMemoryDb();
        var role = new Role("ADMIN");
        db.Roles.Add(role);
        await db.SaveChangesAsync();

        var service = new RoleService(db);

        // when
        var result = await service.GetRoleByName("ADMIN");

        // then
        Assert.NotNull(result);
        Assert.Equal("ADMIN", result.Name);
    }

    [Fact]
    public async Task GetRoleByName_ShouldThrow_WhenNotExists()
    {
        await using var db = GetInMemoryDb();
        var service = new RoleService(db);

        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await service.GetRoleByName("NON_EXISTENT_ROLE");
        });
    }
}
