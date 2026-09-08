namespace DBDRandomizer.Models;

public class RandomizerResult
{
    public Survivor? Survivor { get; set; }
    public Killer? Killer { get; set; }

    public List<Perk?> SurvivorPerks { get; set; } = new();
    public List<Perk?> KillerPerks { get; set; } = new();

    public Item? SurvivorItem { get; set; }

    public List<Addon?> SurvivorAddons { get; set; } = new();
    public List<Addon?> KillerAddons { get; set; } = new();

    public Offering? SurvivorOffering { get; set; }
    public Offering? KillerOffering { get; set; }

    public List<SkinPiece> SurvivorSkin { get; set; } = new();
    public List<SkinPiece> KillerSkin { get; set; } = new();
    
}