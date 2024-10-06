using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Features.Score
{
    [Table("scores")]
    public class ScoreEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User.Entities.User User { get; set; } = null!;

        [Required]
        [Column("score")]
        public int Score { get; set; }

        [Required]
        [Column("created_at", TypeName = "timestamp")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
