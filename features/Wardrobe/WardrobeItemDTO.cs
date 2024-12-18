using System;
using Features.User.DTOs;

namespace Features.Wardrobe.DTOs
{
    public record WardrobeItemDTO
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = null!;
        public int Price { get; init; }
        public Rank RankRequirement { get; init; }
    }

        public record CreateWardrobeItemDTO
    {
        public string Name { get; init; } = null!;
        public int Price { get; init; }
        public Rank RankRequirement { get; init; }
    }
}