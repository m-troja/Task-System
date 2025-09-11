using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Task_System.Data;
using Task_System.Exception.UserException;
using Task_System.Model.Entity;
using Task_System.Config;

namespace Task_System.Service.Impl;

public class UserService : IUserService
{

    private readonly PostgresqlDbContext _db;
    private readonly ILogger<UserService> l;

    public UserService(PostgresqlDbContext db, ILogger<UserService> logger)
    {
        _db = db;
       l = logger;
    }



    public async Task<User> GetByIdAsync(int id)
    {
        l.log($"Fetching user by id {id}");
        User? user = await _db.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
        if (user == null)
        {
            throw new UserNotFoundException("User by id '" + id + "' was not found");
        }
        l.log("User fetched: " + user);
        return user;
    }
    
    public async Task<User> CreateUserAsync(User user)
    {
        l.log("Creating user: " + user);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        Console.WriteLine("User created successfully: " + user);

        return user;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        User? user = await _db.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == email);
        if (user == null)
        {
            throw new UserNotFoundException("User by email '" + email + "' was not found");
        }
        return user;
    }

    public async Task UpdateUserAsync(User user) 
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }
}