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

    /// <summary>
    /// Garantiza que una perk licenciada y su versión general no estén
    /// habilitadas a la vez. A diferencia de Migrate, NO depende de la
    /// versión de la config: se ejecuta en cada carga, así cubre también
    /// perks generales agregadas más adelante a los datos.
    /// Devuelve true si la configuración fue modificada.
    /// </summary>
    public static bool NormalizeLinkedPerks(
        RandomizerConfig config,
        List<Perk> survivorPerks,
        List<Perk> killerPerks)
    {
        bool survivorChanged =
            PerkLinks.Normalize(survivorPerks, config.DisabledSurvivorPerks);

        bool killerChanged =
            PerkLinks.Normalize(killerPerks, config.DisabledKillerPerks);

        return survivorChanged || killerChanged;
    }

    /// <summary>
    /// Quita de las listas "Disabled" los IDs que ya no existen en los datos
    /// (por ejemplo, registros eliminados con la herramienta de diagnóstico).
    /// Devuelve true si la configuración fue modificada.
    /// </summary>
    public static bool PruneUnknownIds(
        RandomizerConfig config,
        List<Survivor> survivors,
        List<Killer> killers,
        List<SkinPiece> survivorSkins,
        List<SkinPiece> killerSkins,
        List<Perk> survivorPerks,
        List<Perk> killerPerks,
        List<Item> items,
        List<Addon> addons,
        List<Offering> offerings)
    {
        bool changed = false;

        changed |= Prune(config.DisabledSurvivors, survivors.Select(x => x.Id));
        changed |= Prune(config.DisabledKillers, killers.Select(x => x.Id));
        changed |= Prune(config.DisabledSurvivorSkins, survivorSkins.Select(x => x.Id));
        changed |= Prune(config.DisabledKillerSkins, killerSkins.Select(x => x.Id));
        changed |= Prune(config.DisabledSurvivorPerks, survivorPerks.Select(x => x.Id));
        changed |= Prune(config.DisabledKillerPerks, killerPerks.Select(x => x.Id));
        changed |= Prune(config.DisabledItems, items.Select(x => x.Id));
        changed |= Prune(config.DisabledAddons, addons.Select(x => x.Id));
        changed |= Prune(config.DisabledOfferings, offerings.Select(x => x.Id));

        return changed;
    }

    private static bool Prune(List<int> ids, IEnumerable<int> existingIds)
    {
        HashSet<int> valid = existingIds.ToHashSet();

        return ids.RemoveAll(id => !valid.Contains(id)) > 0;
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
