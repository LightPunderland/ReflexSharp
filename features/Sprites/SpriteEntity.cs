using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Features.Sprite.Entities
{
    [Table("sprites")]
    public record SpriteEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        [Required]
        public string Name { get; set; }
        [Required]
        [Column("image_data")]
        public byte[] ImageData { get; set; }
    }
}
