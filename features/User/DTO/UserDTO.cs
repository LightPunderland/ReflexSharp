namespace Features.User.DTOs;

// enum
public enum Status{
    Online,
    Offline,
    AFK,
    DND
}

//record
public record UserDTO{
    public Guid Id {get; init;}
    public string Email {get; init;} = null;
    public string DisplayName {get; init; } = null!;
    public Status Activity { get; init; } = Status.Offline;
}
