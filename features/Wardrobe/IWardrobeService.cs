using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Features.Wardrobe.DTOs;

namespace Features.Wardrobe.Services
{
    public interface IWardrobeService
    {
        Task<IEnumerable<WardrobeItemDTO>> GetAllWardrobeItemsAsync();
        Task<WardrobeItemDTO?> GetWardrobeItemAsync(Guid itemId);
        Task<WardrobeItemDTO> CreateWardrobeItemAsync(CreateWardrobeItemDTO itemDto);
        Task<WardrobeItemDTO?> GetWardrobeItemByNameAsync(string name);
    }
}