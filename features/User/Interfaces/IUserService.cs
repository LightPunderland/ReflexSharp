using Features.User.Entities;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();

    Task<User> GetUserAsync(Guid userID);
}

