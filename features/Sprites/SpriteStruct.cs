namespace Features.Sprite.SpriteStruct;

public struct SpriteEntry {
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] ImageData { get; set;}

    public SpriteEntry(int Id, string Name, byte[] ImageData){
        this.Id = Id;
        this.Name = Name;
        this.ImageData = ImageData;
    }
}
