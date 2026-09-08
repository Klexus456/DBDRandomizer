namespace DBDRandomizer.Models;

public class Addon
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int? CharacterId { get; set; }
    public int? ItemId { get; set; }
    public int? AddonGroupId { get; set; }
    public Rarity Rarity { get; set; }
    public string Image { get; set; } = "";
    public string Description { get; set; } = "";
    
}