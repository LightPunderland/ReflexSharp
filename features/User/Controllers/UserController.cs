using Features.User.Entities;
using Microsoft.AspNetCore.Mvc;


[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("api/users")]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("api/users/{userID}")]
    public async Task<ActionResult<User>> GetUser(String userID)
    {
        Guid? guid = null;

        //Patikrina userID formatas valid
        try{
            guid = Guid.Parse(userID);
        } catch (Exception){};

        if(guid is not null){
            var user = await _userService.GetUserAsync(guid.GetValueOrDefault());

            if(user is not null){
                return Ok(user);
            }
            else{
                return NotFound();
            }
        }

        return NotFound();
    }
}
