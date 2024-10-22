using Data;
using Features.User.Entities;
using Microsoft.EntityFrameworkCore;
using Features.User.DTOs;

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
        return users.Select(user => new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName
        });
    }

    public async Task<UserDTO?> GetUserAsync(Guid userID)
    {
        var user = await _context.Users.FindAsync(userID);
        if (user == null) return null;

        return new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            Activity = user.Activity
        };
    }

    public async Task<IEnumerable<UserDTO>> GetOnlineUsersAsync()
    {
        var users = await _context.Users.ToListAsync();
        var onlineUsers = new List<UserDTO>();

        // iterating
        foreach (var user in users)
        {
            if (user.Activity == Status.Online)
            {
                onlineUsers.Add(new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    DisplayName = user.DisplayName
                });
            }
        }

        return onlineUsers;
    }




}
