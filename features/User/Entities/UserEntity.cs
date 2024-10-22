using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Features.User.DTOs;

namespace Features.User.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, Column(TypeName = "varchar(255)")]
        public string GoogleId { get; set; } = null!;

        [Required, Column(TypeName = "varchar(255)")]
        public string Email { get; set; } = null!;

        [Required, Column(TypeName = "varchar(100)")]
        public string DisplayName { get; set; } = null!;

        [Required, Column(TypeName = "varchar(50)")]
        public Status Activity { get; set; } = Status.Offline;
    }
}
