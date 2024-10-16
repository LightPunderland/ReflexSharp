using Microsoft.AspNetCore.Mvc;


[ApiController]
public class SpriteController : ControllerBase
{
    [HttpGet("sprites/ninja")]
    public IActionResult Get()
    {
        FileStream stream = System.IO.File.Open(@"features\Sprites\assets\ninja.png", FileMode.Open);
        return File(stream, "image/jpeg");
    }
}

