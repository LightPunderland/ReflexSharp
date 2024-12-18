using System;

namespace Features.Wardrobe.DTOs
{
    public record PurchaseEligibilityDTO
    {
        public bool HasSufficientGold { get; init; }
        public bool MeetsRankRequirement { get; init; }
        public bool IsEligibleForPurchase => HasSufficientGold && MeetsRankRequirement;
        public string? Message { get; init; }
    }
}