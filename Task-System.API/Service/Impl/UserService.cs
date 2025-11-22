using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using Task_System.Data;
using Task_System.Exception.UserException;
using Task_System.Model.Entity;
using Task_System.Log;
using Task_System.Model.DTO;
using Task_System.Model.DTO.Cnv;

namespace Task_System.Service.Impl;

public class UserService : IUserService
{
    private string SlackBotSlackUserId = "USLACKBOT";
    private readonly PostgresqlDbContext _db;
    private readonly ILogger<UserService> l;
    private readonly UserCnv _userCnv;
    private readonly IChatGptService _chatGptService;

    public UserService(PostgresqlDbContext db, ILogger<UserService> logger, UserCnv userCnv, IChatGptService _chatGptService)
    {
        _db = db;
        l = logger;
        _userCnv = userCnv;
        this._chatGptService = _chatGptService;
    }
    public async Task<User> GetByIdAsync(int id)
    {
        l.LogDebug($"Fetching user by id {id}");
        User? user = await _db.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id) ?? throw new UserNotFoundException("User by id '" + id + "' was not found");
        l.LogDebug("User fetched: " + user);
        return user;
    }
    
    public async Task<User> CreateUserAsync(User user)
    {
        l.LogDebug("Creating user: " + user);
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
            l.LogError("User by email '" + email + "' was not found");
            return null;
        }
        return user;
    }
    public async Task<User?> TryGetByEmailAsync(string email)
    {
        User? user = await _db.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == email);
        
        return user;
    }

    public async Task UpdateUserAsync(User user) 
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        l.LogDebug("Fetching all uses from db");
        List<User> users = await _db.Users.Include(u => u.Roles).ToListAsync();
        l.LogDebug($"Fetched {users.Count} users");
        return users;
    }

    public async Task<UserDto> GetUserBySlackUserIdAsync(string slackUserId)
    {
        l.LogDebug($"Fetching user by Slack user ID: {slackUserId}");
        User? user = await _db.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.SlackUserId == slackUserId);
        if (user == null)
        {
            l.LogDebug($"User with Slack user ID '{slackUserId}' not found");
            throw new UserNotFoundException("User by Slack user ID '" + slackUserId + "' was not found");
        }
        l.LogDebug("User fetched: " + user);
        UserDto userDto = _userCnv.ConvertUserToDto(user);
        return userDto;
    }

    public async Task<int> GetIdBySlackUserId(string slackUserId)
    {
        if (string.IsNullOrEmpty(slackUserId)) { throw new ArgumentException("Slack user ID cannot be null or empty", slackUserId); }
        l.LogDebug($"Getting user ID by Slack user ID: {slackUserId}");

        int id = await _db.Users.Where(u => u.SlackUserId == slackUserId)
            .Select(u => u.Id)
            .FirstOrDefaultAsync();
        l.LogDebug($"First fetch of user ID: {id} for Slack user ID: {slackUserId}");


        if (id == 0)
        {
            l.LogDebug($"User with Slack user ID '{slackUserId}' not found - calling ChatGPT API");
            var users = await _chatGptService.GetAllChatGptUsersAsync();
            id = users.Find(u => u.SlackUserId == slackUserId)?.Id ?? 0;
            l.LogDebug($"Second fetch of user ID: {id} for Slack user ID: {slackUserId}"); 
            if (id != 0)
            {
                l.LogDebug($"User with Slack user ID '{slackUserId}' found after ChatGPT sync: ID {id}");
                return id;
            }
        }

        if (id == 0)
        {
            l.LogDebug($"User with Slack user ID '{slackUserId}' not found - assigning bot");
            var BotUser = await _db.Users.Where(u => u.SlackUserId == SlackBotSlackUserId).FirstAsync();
            l.LogDebug($"Bot fetched: {BotUser}");
            return BotUser.Id;
        }
        return id;
    }
    public async Task DeleteAllUsers()
    {
        await _db.Database.ExecuteSqlRawAsync("DELETE FROM Users");
        l.LogInformation("Deleted all Users from database");
    }

    public async Task DeleteUserById(int id)
    {
        await _db.Database.ExecuteSqlAsync($"DELETE FROM Users WHERE id = {id}");
        l.LogInformation($"Deleted User by Id={id}");
    }
}