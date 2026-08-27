namespace DBDRandomizer.Models;

public class Offering
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public Rarity Rarity { get; set; }
    public string Image { get; set; } = "";
    public string Role { get; set; } = "Both";
}