using Features.User.Entities;
using Features.User.DTOs;

public interface IUserService
{
    Task<IEnumerable<UserDTO>> GetAllUsersAsync();

    Task<UserDTO?> GetUserAsync(Guid userID);

    Task<IEnumerable<UserDTO>> GetUsersByRankAsync(Rank rank);
    Task<bool> CheckUsernameAvailabilityAsync(string username);

    // Used for displaying how much xp is needed for a new level on the frontend
    Task<int> CheckXPRequiredForLevelUp(Guid userID);


}

