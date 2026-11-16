using DBDRandomizer.Models;

namespace DBDRandomizer.Randomizer;

public static class ConfigMigrator
{
    public const int CurrentVersion = 1;

    /// <summary>
    /// Actualiza una configuración vieja al formato actual.
    /// Devuelve true si la configuración fue modificada
    /// (en ese caso conviene volver a guardarla).
    /// </summary>
    public static bool Migrate(
        RandomizerConfig config,
        List<Perk> survivorPerks,
        List<Perk> killerPerks)
    {
        if (config.ConfigVersion >= CurrentVersion)
        {
            return false;
        }

        // Versión 0 -> 1:
        // Antes, las perks de un personaje deshabilitado se excluían
        // automáticamente. Ahora el estado de cada perk es explícito,
        // así que las perks de los personajes ya deshabilitados se
        // pasan a la lista de perks deshabilitadas.
        DisablePerksOfCharacters(
            survivorPerks,
            config.DisabledSurvivors,
            config.DisabledSurvivorPerks
        );

        DisablePerksOfCharacters(
            killerPerks,
            config.DisabledKillers,
            config.DisabledKillerPerks
        );

        config.ConfigVersion = CurrentVersion;

        return true;
    }

    private static void DisablePerksOfCharacters(
        List<Perk> perks,
        List<int> disabledCharacterIds,
        List<int> disabledPerkIds)
    {
        foreach (Perk perk in perks)
        {
            if (perk.CharacterId.HasValue &&
                disabledCharacterIds.Contains(perk.CharacterId.Value) &&
                !disabledPerkIds.Contains(perk.Id))
            {
                disabledPerkIds.Add(perk.Id);
            }
        }
    }
}
