using Features.User.Entities;
using Features.User.DTOs;

public interface IUserService
{
    Task<IEnumerable<UserDTO>> GetAllUsersAsync();

    Task<UserDTO?> GetUserAsync(Guid userID);

    Task<IEnumerable<UserDTO>> GetUsersByRankAsync(Rank rank);
    Task<bool> CheckUsernameAvailabilityAsync(string username);

    // Working on this, doesn't do much for now
    Task<int> CheckXPRequiredForLevelUp(Guid userID);

    Task<bool> UpdateUserGoldXp(Guid userId, int gold, int xp);
    Task<User?> ValidateUserAsync(string googleId, string email, string displayName);
}

