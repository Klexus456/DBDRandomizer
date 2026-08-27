using DBDRandomizer.Models;

namespace DBDRandomizer.Randomizer;

public class ConfigValidator
{
    private readonly List<Survivor> _survivors;
    private readonly List<Killer> _killers;
    private readonly List<Item> _items;
    private readonly List<Addon> _addons;
    private readonly List<Perk> _survivorPerks;
    private readonly List<Perk> _killerPerks;
    private readonly RandomizerConfig _config;
    private readonly List<SkinPiece> _survivorSkins;
    private readonly List<SkinPiece> _killerSkins;
    private readonly List<Offering> _offerings;

    public ConfigValidator(
        List<Survivor> survivors,
        List<SkinPiece> survivorSkins,
        List<Killer> killers,
        List<SkinPiece> killerSkins,
        List<Item> items,
        List<Addon> addons,
        List<Perk> survivorPerks,
        List<Perk> killerPerks,
        List<Offering> offerings,
        RandomizerConfig config)
    {
        _survivors = survivors;
        _survivorSkins = survivorSkins;

        _killers = killers;
        _killerSkins = killerSkins;

        _items = items;
        _addons = addons;
        _offerings = offerings;

        _survivorPerks = survivorPerks;
        _killerPerks = killerPerks;

        _config = config;
    }

    public List<string> Validate()
    {
        List<string> errors = new();

        ValidateSurvivors(errors);
        ValidateKillers(errors);

        ValidateSurvivorPerks(errors);
        ValidateKillerPerks(errors);

        ValidateSurvivorItems(errors);
        ValidateOfferings(errors);

        ValidateKillerAddons(errors);

        ValidateSurvivorSkins(errors);
        ValidateKillerSkins(errors);

        ValidateDuplicateIds(errors);
        ValidateExistingIds(errors);

        return errors;
    }


    // =========================================================
    // DUPLICADOS
    // =========================================================

    private void ValidateDuplicateIds(List<string> errors)
    {
        ValidateDuplicateIds(
            _config.DisabledSurvivors,
            "Survivors deshabilitados",
            errors
        );

        ValidateDuplicateIds(
            _config.DisabledKillers,
            "Killers deshabilitados",
            errors
        );

        ValidateDuplicateIds(
            _config.DisabledSurvivorSkins,
            "Skins de Survivor deshabilitadas",
            errors
        );

        ValidateDuplicateIds(
            _config.DisabledKillerSkins,
            "Skins de Killer deshabilitadas",
            errors
        );

        ValidateDuplicateIds(
            _config.DisabledSurvivorPerks,
            "Perks de Survivor deshabilitadas",
            errors
        );

        ValidateDuplicateIds(
            _config.DisabledKillerPerks,
            "Perks de Killer deshabilitadas",
            errors
        );

        ValidateDuplicateIds(
            _config.DisabledItems,
            "Items deshabilitados",
            errors
        );

        ValidateDuplicateIds(
            _config.DisabledAddons,
            "Addons deshabilitados",
            errors
        );

        ValidateDuplicateIds(
            _config.DisabledOfferings,
            "Offerings deshabilitadas",
            errors
        );
    }


    // =========================================================
    // IDS EXISTENTES
    // =========================================================

    private void ValidateExistingIds(List<string> errors)
    {
        ValidateExistingIds(
            _config.DisabledSurvivors,
            _survivors.Select(s => s.Id),
            "Survivors",
            errors
        );

        ValidateExistingIds(
            _config.DisabledKillers,
            _killers.Select(k => k.Id),
            "Killers",
            errors
        );

        ValidateExistingIds(
            _config.DisabledSurvivorSkins,
            _survivorSkins.Select(s => s.Id),
            "Skins de Survivor",
            errors
        );

        ValidateExistingIds(
            _config.DisabledKillerSkins,
            _killerSkins.Select(s => s.Id),
            "Skins de Killer",
            errors
        );

        ValidateExistingIds(
            _config.DisabledSurvivorPerks,
            _survivorPerks.Select(p => p.Id),
            "Perks de Survivor",
            errors
        );

        ValidateExistingIds(
            _config.DisabledKillerPerks,
            _killerPerks.Select(p => p.Id),
            "Perks de Killer",
            errors
        );

        ValidateExistingIds(
            _config.DisabledItems,
            _items.Select(i => i.Id),
            "Items",
            errors
        );

        ValidateExistingIds(
            _config.DisabledAddons,
            _addons.Select(a => a.Id),
            "Addons",
            errors
        );

        ValidateExistingIds(
        _config.DisabledOfferings,
        _offerings.Select(o => o.Id),
        "Offerings",
        errors
        );
    }


    private void ValidateExistingIds(
        List<int> configuredIds,
        IEnumerable<int> existingIds,
        string category,
        List<string> errors)
    {
        HashSet<int> existingIdSet = existingIds.ToHashSet();

        foreach (int id in configuredIds)
        {
            if (!existingIdSet.Contains(id))
            {
                errors.Add(
                    $"El ID {id} de {category} no existe en los datos."
                );
            }
        }
    }


    // =========================================================
    // DUPLICADOS
    // =========================================================

    private void ValidateDuplicateIds(
        List<int> ids,
        string category,
        List<string> errors)
    {
        List<int> duplicatedIds = ids
            .GroupBy(id => id)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        foreach (int id in duplicatedIds)
        {
            errors.Add(
                $"La configuración de {category} contiene el ID {id} más de una vez."
            );
        }
    }


    // =========================================================
    // SURVIVORS
    // =========================================================

    private void ValidateSurvivors(List<string> errors)
    {
        int availableSurvivors = _survivors
            .Count(survivor =>
                !_config.DisabledSurvivors.Contains(survivor.Id));

        if (availableSurvivors < 2)
        {
            errors.Add(
                "Debe haber al menos 2 Survivors disponibles."
            );
        }
    }


    // =========================================================
    // KILLERS
    // =========================================================

    private void ValidateKillers(List<string> errors)
    {
        int availableKillers = _killers
            .Count(killer =>
                !_config.DisabledKillers.Contains(killer.Id));

        if (availableKillers < 2)
        {
            errors.Add(
                $"Hay solamente {availableKillers} Killers disponibles. " +
                "Debe haber al menos 2."
            );
        }
    }


    // =========================================================
    // SURVIVOR PERKS
    // =========================================================

    private void ValidateSurvivorPerks(List<string> errors)
    {
        int availablePerks = _survivorPerks
            .Count(perk =>
                !_config.DisabledSurvivorPerks.Contains(perk.Id) &&
                (
                    perk.CharacterId == null ||
                    !_config.DisabledSurvivors.Contains(
                        perk.CharacterId.Value
                    )
                ));

        if (availablePerks < 4)
        {
            errors.Add(
                $"Hay solamente {availablePerks} perks de Survivor disponibles. " +
                "Debe haber al menos 4."
            );
        }
    }


    // =========================================================
    // KILLER PERKS
    // =========================================================

    private void ValidateKillerPerks(List<string> errors)
    {
        int availablePerks = _killerPerks
            .Count(perk =>
                !_config.DisabledKillerPerks.Contains(perk.Id) &&
                (
                    perk.CharacterId == null ||
                    !_config.DisabledKillers.Contains(
                        perk.CharacterId.Value
                    )
                ));

        if (availablePerks < 4)
        {
            errors.Add(
                $"Hay solamente {availablePerks} perks de Killer disponibles. " +
                "Debe haber al menos 4."
            );
        }
    }


    // =========================================================
    // SURVIVOR ITEMS / ADDONS
    // =========================================================

    private void ValidateSurvivorItems(List<string> errors)
    {
        List<Item> availableItems = _items
            .Where(item =>
                !_config.DisabledItems.Contains(item.Id))
            .ToList();

        if (availableItems.Count < 2)
        {
            errors.Add(
                $"Hay solamente {availableItems.Count} Items disponibles. " +
                "Debe haber al menos 2."
            );

            return;
        }

        foreach (Item item in availableItems)
        {
            int availableAddons = _addons
                .Count(addon =>
                    addon.ItemId == item.Id &&
                    !_config.DisabledAddons.Contains(addon.Id));

            if (availableAddons < 3)
            {
                errors.Add(
                    $"El Item '{item.Name}' tiene solamente " +
                    $"{availableAddons} Add-on(s) disponibles. " +
                    "Debe tener al menos 3."
                );
            }
        }
    }

    private void ValidateKillerAddons(List<string> errors)
    {
        List<Killer> availableKillers = _killers
            .Where(killer =>
                !_config.DisabledKillers.Contains(killer.Id))
            .ToList();

        foreach (Killer killer in availableKillers)
        {
            int availableAddons = _addons
                .Count(addon =>
                    addon.CharacterId == killer.Id &&
                    !_config.DisabledAddons.Contains(addon.Id));

            if (availableAddons < 3)
            {
                errors.Add(
                    $"El Killer '{killer.Name}' tiene solamente " +
                    $"{availableAddons} Add-on(s) disponibles. " +
                    "Debe tener al menos 3."
                );
            }
        }
    }

    private void ValidateOfferings(List<string> errors)
    {
        int availableSurvivorOfferings = _offerings
            .Count(offering =>
                !_config.DisabledOfferings.Contains(offering.Id) &&
                (
                    offering.Role == "Survivor" ||
                    offering.Role == "Both"
                ));

        if (availableSurvivorOfferings < 3)
        {
            errors.Add(
                $"Hay solamente {availableSurvivorOfferings} Offerings de Survivor disponibles. " +
                "Debe haber al menos 3."
            );
        }


        int availableKillerOfferings = _offerings
            .Count(offering =>
                !_config.DisabledOfferings.Contains(offering.Id) &&
                (
                    offering.Role == "Killer" ||
                    offering.Role == "Both"
                ));

        if (availableKillerOfferings < 3)
        {
            errors.Add(
                $"Hay solamente {availableKillerOfferings} Offerings de Killer disponibles. " +
                "Debe haber al menos 3."
            );
        }
    }

    // =========================================================
    // SURVIVOR SKINS
    // =========================================================

    private void ValidateSurvivorSkins(List<string> errors)
    {
        List<Survivor> availableSurvivors = _survivors
            .Where(survivor =>
                !_config.DisabledSurvivors.Contains(survivor.Id))
            .ToList();

        foreach (Survivor survivor in availableSurvivors)
        {
            List<SkinPiece> availableSkins = _survivorSkins
                .Where(skin =>
                    skin.CharacterId == survivor.Id &&
                    !_config.DisabledSurvivorSkins.Contains(skin.Id))
                .ToList();

            if (availableSkins.Count == 0)
            {
                continue;
            }

            // Validar outfits inseparables
            List<int> inseparableOutfits = availableSkins
                .Where(skin => !skin.IsSeparable)
                .Select(skin => skin.OutfitId)
                .Distinct()
                .ToList();

            foreach (int outfitId in inseparableOutfits)
            {
                List<SkinPiece> outfitPieces = availableSkins
                    .Where(skin => skin.OutfitId == outfitId)
                    .ToList();

                bool hasHead = outfitPieces
                    .Any(skin => skin.Part == SkinPart.Head);

                bool hasTorso = outfitPieces
                    .Any(skin => skin.Part == SkinPart.Torso);

                bool hasLegs = outfitPieces
                    .Any(skin => skin.Part == SkinPart.Legs);

                if (!hasHead || !hasTorso || !hasLegs)
                {
                    errors.Add(
                        $"El outfit inseparable '{outfitId}' de " +
                        $"'{survivor.Name}' no tiene todas sus partes disponibles."
                    );
                }
            }
        }
    }


    // =========================================================
    // KILLER SKINS
    // =========================================================

    private void ValidateKillerSkins(List<string> errors)
    {

        if (!HasValidKillerOutfit())
        {
            errors.Add(
                "Debe haber al menos 1 outfit de Killer disponible."
            );
        }
        List<Killer> availableKillers = _killers
            .Where(killer =>
                !_config.DisabledKillers.Contains(killer.Id))
            .ToList();

        foreach (Killer killer in availableKillers)
        {
            List<SkinPiece> availableSkins = _killerSkins
                .Where(skin =>
                    skin.CharacterId == killer.Id &&
                    !_config.DisabledKillerSkins.Contains(skin.Id))
                .ToList();

            List<int> inseparableOutfits = availableSkins
                .Where(skin => !skin.IsSeparable)
                .Select(skin => skin.OutfitId)
                .Distinct()
                .ToList();

            foreach (int outfitId in inseparableOutfits)
            {
                List<SkinPiece> outfitPieces = availableSkins
                    .Where(skin => skin.OutfitId == outfitId)
                    .ToList();

                bool hasHead = outfitPieces
                    .Any(skin => skin.Part == SkinPart.Head);

                bool hasTorso = outfitPieces
                    .Any(skin => skin.Part == SkinPart.Torso);

                bool hasWeapon = outfitPieces
                    .Any(skin => skin.Part == SkinPart.Weapon);

                if (!hasHead || !hasTorso || !hasWeapon)
                {
                    errors.Add(
                        $"El outfit inseparable '{outfitId}' de " +
                        $"'{killer.Name}' no tiene todas sus partes disponibles."
                    );
                }
            }
        }
    }

    private bool HasValidKillerOutfit()
    {
        foreach (Killer killer in _killers)
        {
            if (_config.DisabledKillers.Contains(killer.Id))
                continue;

            List<SkinPiece> availableSkins = _killerSkins
                .Where(skin =>
                    skin.CharacterId == killer.Id &&
                    !_config.DisabledKillerSkins.Contains(skin.Id))
                .ToList();

            IEnumerable<IGrouping<int, SkinPiece>> outfits =
                availableSkins.GroupBy(skin => skin.OutfitId);

            foreach (IGrouping<int, SkinPiece> outfit in outfits)
            {
                if (outfit.All(skin => skin.IsSeparable))
                {
                    if (outfit.Any())
                        return true;
                }

                bool hasHead = outfit.Any(skin => skin.Part == SkinPart.Head);
                bool hasTorso = outfit.Any(skin => skin.Part == SkinPart.Torso);
                bool hasWeapon = outfit.Any(skin => skin.Part == SkinPart.Weapon);

                if (hasHead && hasTorso && hasWeapon)
                    return true;
            }
        }

        return false;
    }

}