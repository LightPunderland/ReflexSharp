namespace Features.User.DTOs;

// enum
public enum Rank
{
        None,
    Noob,
    Pro,
    Master,
    God,
    Admin
}

// Record
public record UserDTO
{
    public Guid Id { get; init; }
    public string GoogleId { get; init; } = null!;
    public string Email { get; init; } = null;
    public string DisplayName { get; init; } = null!;
    public Rank PublicRank { get; init; } = Rank.None;

    public Int32 XP { get; init; } = 0;
    public int Gold { get; init; } = 0;

    public string EquippedSkin { get; init; } = "Ninja";
    public IEnumerable<string> OwnedSkins { get; init; } = new[] { "Ninja" };    
}
