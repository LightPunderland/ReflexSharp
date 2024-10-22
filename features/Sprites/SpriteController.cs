using Data;
using Features.Sprite.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class SpriteController : ControllerBase
{
    private readonly AppDbContext _context;

    public SpriteController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadSprite(IFormFile file, string name)
    {
    if (file == null || file.Length == 0)
        return BadRequest("No file uploaded.");

    using (var memoryStream = new MemoryStream())
    {
        await file.CopyToAsync(memoryStream);
        var sprite = new SpriteEntity
        {
            Name = name,
            ImageData = memoryStream.ToArray()
        };

        _context.Sprites.Add(sprite);
        await _context.SaveChangesAsync();
    }

    return Ok(new { message = "Sprite uploaded successfully!" });
}


    [HttpGet("{id}")]
    public async Task<IActionResult> GetSprite(string id)
    {
        var sprite = await _context.Sprites.FindAsync(id);
        if (sprite == null) return NotFound();

        return File(sprite.ImageData, "image/png");
    }

    [HttpGet("by-name/{name}")]
    public async Task<IActionResult> GetSpriteByName(string name)
    {
        var sprite = await _context.Sprites.FirstOrDefaultAsync(s => s.Name == name);
        if (sprite == null) return NotFound();

        return File(sprite.ImageData, "image/png");
    }
}
