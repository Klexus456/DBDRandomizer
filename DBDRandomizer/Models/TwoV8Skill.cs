namespace DBDRandomizer.Models;

public class TwoV8Skill
{
    public int Id { get; set; }

    public string Role { get; set; } = "";

    public int? ClassId { get; set; }

    public int? CharacterId { get; set; }

    public string Type { get; set; } = "";

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public string Image { get; set; } = "";

    public int Slot { get; set; }
}