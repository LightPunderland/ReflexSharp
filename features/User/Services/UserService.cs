using Data;
using Features.User.Entities;
using Features.User.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class UserService : IUserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        var userList = new UserList(users);

        userList.Sort();

        return userList.Select(user => new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            PublicRank = user.Rank
        }).ToList();
    }

    public async Task<UserDTO?> GetUserAsync(Guid userID)
    {
        var user = await _context.Users.FindAsync(userID);

        if (user is null)
            return null;

        return new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            XP = user.XP,
            Coins = user.Coins,
            DisplayName = user.DisplayName,
            PublicRank = user.Rank
        };
    }

    public async Task<IEnumerable<UserDTO>> GetUsersByRankAsync(Rank rank)
    {
        var users = await _context.Users.ToListAsync();
        var userList = new UserList(users);

        var filteredUsers = userList
            .Where(u => u.Rank == rank)
            .ToList();


        filteredUsers.Sort();

        return filteredUsers.Select(user => new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            XP = user.XP,
            Coins = user.Coins,
            DisplayName = user.DisplayName,
            PublicRank = rank
        }).ToList();
    }

    public async Task<bool> CheckUsernameAvailabilityAsync(string username)
    {
        var users = await _context.Users.ToListAsync();
        var userList = new UserList(users);

        return userList.All(u => u.Email != username);
    }

    // rank formula, when even 125*x^3 and 125*x^3 + 125 when odd, x is user rank enum
    public async Task<int> CheckXPRequiredForLevelUp(Guid userID) {

        try
        {
            var user = await _context.Users.FindAsync(userID) ?? throw new Exception("User not found.");
            int rankVariable = (int)user.Rank;
            return (rankVariable % 2 != 0) ? 125*(rankVariable)^3 : 125*(rankVariable)^3 + 125;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
            return -1;
        }

    }
}
