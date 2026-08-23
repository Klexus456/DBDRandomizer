namespace DBDRandomizer.Models;

public class RandomizerConfig
{
    // =========================
    // PERSONAJES DESHABILITADOS
    // =========================

    public List<int> DisabledSurvivors { get; set; } = new();

    public List<int> DisabledKillers { get; set; } = new();


    // =========================
    // SKINS DESHABILITADAS
    // =========================

    public List<int> DisabledSurvivorSkins { get; set; } = new();

    public List<int> DisabledKillerSkins { get; set; } = new();


    // =========================
    // PERKS DESHABILITADAS
    // =========================

    public List<int> DisabledSurvivorPerks { get; set; } = new();

    public List<int> DisabledKillerPerks { get; set; } = new();


    // =========================
    // ITEMS DESHABILITADOS
    // =========================

    public List<int> DisabledItems { get; set; } = new();


    // =========================
    // ADDONS DESHABILITADOS
    // =========================

    public List<int> DisabledAddons { get; set; } = new();


    // =========================
    // OFRENDAS DESHABILITADAS
    // =========================

    public List<int> DisabledOfferings { get; set; } = new();


    // =========================
    // CONFIGURACIÓN DE SKINS
    // =========================

    public SkinRandomizationMode SurvivorSkinMode { get; set; }
        = SkinRandomizationMode.Parts;

    public SkinRandomizationMode KillerSkinMode { get; set; }
        = SkinRandomizationMode.Parts;
}