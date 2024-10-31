using Features.User.Entities;
using Features.User.DTOs;

public interface IUserService
{
    Task<IEnumerable<UserDTO>> GetAllUsersAsync();

    Task<UserDTO?> GetUserAsync(Guid userID);


    Task<IEnumerable<UserDTO>> GetUsersByRankAsync(Rank rank);
    Task<bool> CheckUsernameAvailabilityAsync(string username);

   Task<int> CheckXPRequiredForLevelUp(Guid userID);

}

