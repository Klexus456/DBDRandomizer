namespace DBDRandomizer.Models;

public class Perk
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int? CharacterId { get; set; }
    public string Description { get; set; } = "";
    public string Image { get; set; } = "";
}