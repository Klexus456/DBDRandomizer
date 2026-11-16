namespace DBDRandomizer.Models;

public class RandomizerConfig
{
    // =========================
    // VERSIÓN DE LA CONFIGURACIÓN
    // =========================

    // 0 = configuración anterior: las perks de un personaje
    //     deshabilitado se excluían automáticamente en el motor.
    // 1 = las perks son independientes: su estado se guarda
    //     explícitamente en DisabledSurvivorPerks / DisabledKillerPerks.
    public int ConfigVersion { get; set; } = 0;


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

    // ============================
    // CONFIGURACION SLOTS VACIOS
    // ============================

    public int PerkEmptyChance { get; set; } = 0;
    public int ItemEmptyChance { get; set; } = 0;
    public int AddonEmptyChance { get; set; } = 0;
    public int OfferingEmptyChance { get; set; } = 0;
}