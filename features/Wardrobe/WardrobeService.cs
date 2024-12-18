using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Features.Wardrobe.DTOs;
using Features.Wardrobe.Entities;
using Features.User.DTOs;
using Data;

namespace Features.Wardrobe.Services
{
    public class WardrobeService : IWardrobeService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;

        public WardrobeService(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<IEnumerable<WardrobeItemDTO>> GetAllWardrobeItemsAsync()
        {
            var items = await _context.WardrobeItems.ToListAsync();
            return items.Select(item => new WardrobeItemDTO
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                RankRequirement = item.RequiredRank
            });
        }

        public async Task<WardrobeItemDTO?> GetWardrobeItemAsync(Guid itemId)
        {
            var item = await _context.WardrobeItems.FindAsync(itemId);
            if (item == null) return null;

            return new WardrobeItemDTO
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                RankRequirement = item.RequiredRank
            };
        }

        public async Task<PurchaseEligibilityDTO> CheckPurchaseEligibilityAsync(Guid userId, Guid itemId)
        {
            var user = await _userService.GetUserAsync(userId);
            var item = await GetWardrobeItemAsync(itemId);

            if (user == null)
            {
                return new PurchaseEligibilityDTO
                {
                    HasSufficientGold = false,
                    MeetsRankRequirement = false,
                    Message = "User not found"
                };
            }

            if (item == null)
            {
                return new PurchaseEligibilityDTO
                {
                    HasSufficientGold = false,
                    MeetsRankRequirement = false,
                    Message = "Item not found"
                };
            }

            bool hasEnoughGold = user.Gold >= item.Price;
            bool hasRequiredRank = user.PublicRank >= item.RankRequirement;

            string? message = null;
            if (!hasEnoughGold && !hasRequiredRank)
                message = "Insufficient gold and rank";
            else if (!hasEnoughGold)
                message = "Insufficient gold";
            else if (!hasRequiredRank)
                message = "Insufficient rank";

            return new PurchaseEligibilityDTO
            {
                HasSufficientGold = hasEnoughGold,
                MeetsRankRequirement = hasRequiredRank,
                Message = message
            };
        }

        public async Task<WardrobeItemDTO> CreateWardrobeItemAsync(CreateWardrobeItemDTO itemDto)
        {
            var wardrobeItem = new WardrobeItem
            {
                Name = itemDto.Name,
                Price = itemDto.Price,
                RequiredRank = itemDto.RankRequirement
            };

            _context.WardrobeItems.Add(wardrobeItem);
            await _context.SaveChangesAsync();

            return new WardrobeItemDTO
            {
                Id = wardrobeItem.Id,
                Name = wardrobeItem.Name,
                Price = wardrobeItem.Price,
                RankRequirement = wardrobeItem.RequiredRank
            };
        }

        public async Task<WardrobeItemDTO?> GetWardrobeItemByNameAsync(string name)
        {
            var item = await _context.WardrobeItems.FirstOrDefaultAsync(w => w.Name == name);
            if (item == null) return null;

            return new WardrobeItemDTO
            {
                Id = item.Id,
                Name = item.Name,
                Price = item.Price,
                RankRequirement = item.RequiredRank
            };
        }
    }
}