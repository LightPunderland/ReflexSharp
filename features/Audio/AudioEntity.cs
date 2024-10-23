using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Features.Audio.Entities
{
    [Table("audiofiles")]
    public record AudioFileEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("name")]
        [Required]
        public string Name { get; set; }
        [Required]
        [Column("file_data")]
        public byte[] FileData { get; set; }
    }
}
