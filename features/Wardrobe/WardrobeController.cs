using Microsoft.AspNetCore.Mvc;
using Features.Wardrobe.DTOs;
using Features.Wardrobe.Services;
using Features.User.DTOs;

[ApiController]
public class WardrobeController : ControllerBase
{
    private readonly IWardrobeService _wardrobeService;

    public WardrobeController(IWardrobeService wardrobeService)
    {
        _wardrobeService = wardrobeService;
    }

    [HttpGet("api/wardrobe")]
    public async Task<ActionResult<IEnumerable<WardrobeItemDTO>>> GetAllItems()
    {
        var items = await _wardrobeService.GetAllWardrobeItemsAsync();
        return Ok(items);
    }


    [HttpPost("api/wardrobe")]
    public async Task<ActionResult<WardrobeItemDTO>> CreateWardrobeItem([FromBody] CreateWardrobeItemDTO itemDto)
    {
        if (itemDto.Price < 0)
        {
            return BadRequest("Price cannot be negative");
        }

        try
        {
            var createdItem = await _wardrobeService.CreateWardrobeItemAsync(itemDto);
            return CreatedAtAction(nameof(GetAllItems), new { id = createdItem.Id }, createdItem);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred while creating the wardrobe item: {ex.Message}");
        }
    }

    [HttpGet("api/wardrobe/name/{name}")]
    public async Task<ActionResult<WardrobeItemDTO>> GetWardrobeItemByName(string name)
    {
        var item = await _wardrobeService.GetWardrobeItemByNameAsync(name);
        
        if (item == null)
        {
            return NotFound($"Wardrobe item with name '{name}' not found");
        }

        return Ok(item);
    }
}