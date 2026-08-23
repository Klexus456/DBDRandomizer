namespace DBDRandomizer.Models;

public class SkinPiece
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int CharacterId { get; set; }
    public int OutfitId { get; set; }
    public SkinPart Part { get; set; }
    public string Image { get; set; } = "";
    public bool IsSeparable { get; set; } = true;
}