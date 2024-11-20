namespace Features.User.DTOs
{
    public class UserValidationDTO
    {
        public string GoogleId { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
    }
}
