namespace Features.Sprite.Entities
{
    public class SpriteEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] ImageData { get; set; } // Store image data as binary
    }
}
