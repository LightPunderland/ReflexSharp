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
        var userList = new UserList(users);
        return userList.GetUserListDTO();
    }

    public async Task<UserDTO?> GetUserAsync(Guid userID)
    {
        var user = await _context.Users.FindAsync(userID);
        if (user == null) return null;

        var userList = new UserList(new List<User> { user });

        return userList.GetUserListDTO().FirstOrDefault();
    }

    public async Task<IEnumerable<UserDTO>> GetUsersByRankAsync(Rank rank)
    {
        var users = await _context.Users.ToListAsync();
        var userList = new UserList(users);
        return userList.GetUserListByRank(rank).Select(user => new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            PublicRank = Enum.TryParse(user.Rank, out Rank parsedRank) ? parsedRank : Rank.Noob
        });
    }

    public async Task<bool> CheckUsernameAvailabilityAsync(string username)
    {
        var users = await _context.Users.ToListAsync();
        var userList = new UserList(users);
        return userList.CheckUsernameAvailability(username);
    }

}
