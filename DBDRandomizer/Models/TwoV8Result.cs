namespace DBDRandomizer.Models;

public class TwoV8Result
{
    public Survivor? Survivor { get; set; }

    public Killer? Killer { get; set; }

    public TwoV8Class? Class { get; set; }

    public List<TwoV8Skill> Skills { get; set; } = new();

    public TwoV8Skill? KillerInnateSkill { get; set; }
}