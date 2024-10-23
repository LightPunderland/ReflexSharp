namespace Features.User.DTOs;

// enum
public enum Rank{
    None,
    Noob,
    Pro,
    Master,
    God,
    Admin
}

//record
public record UserDTO{
    public Guid Id {get; init;}
    public string Email {get; init;} = null;
    public string DisplayName {get; init; } = null!;
    public Rank PublicRank { get; init; } = Rank.Noob;
}
