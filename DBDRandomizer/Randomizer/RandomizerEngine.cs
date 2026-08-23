using DBDRandomizer.Models;

namespace DBDRandomizer.Randomizer;

public class RandomizerEngine
{
    private readonly Random _random = new();

    private readonly List<Survivor> _survivors;
    private readonly List<SkinPiece> _survivorSkins;
    private readonly RandomizerConfig _config;
    private readonly List<Perk> _survivorPerks;
    private readonly List<Item> _items;
    private readonly List<Addon> _addons;
    private readonly List<Offering> _offerings;
    private readonly List<Killer> _killers;
    private readonly List<SkinPiece> _killerSkins;
    private readonly List<Perk> _killerPerks;

    public RandomizerEngine(
        List<Survivor> survivors,
        List<Killer> killers,
        List<SkinPiece> survivorSkins,
        List<SkinPiece> killerSkins,
        List<Perk> survivorPerks,
        List<Perk> killerPerks,
        List<Item> items,
        List<Addon> addons,
        List<Offering> offerings,
        RandomizerConfig config)
    {
        _survivors = survivors;
        _killers = killers;

        _survivorSkins = survivorSkins;
        _killerSkins = killerSkins;

        _survivorPerks = survivorPerks;
        _killerPerks = killerPerks;

        _items = items;
        _addons = addons;
        _offerings = offerings;

        _config = config;
    }


    // =========================================================
    // SURVIVOR
    // =========================================================

    public Survivor RandomizeSurvivor()
    {
        List<Survivor> availableSurvivors = _survivors
            .Where(survivor =>
                !_config.DisabledSurvivors.Contains(survivor.Id))
            .ToList();

        if (availableSurvivors.Count == 0)
        {
            throw new InvalidOperationException(
                "No hay Survivors disponibles en la configuración."
            );
        }

        return availableSurvivors[
            _random.Next(availableSurvivors.Count)
        ];
    }


    // =========================================================
    // SURVIVOR SKINS
    // =========================================================

    public List<SkinPiece> RandomizeSurvivorSkin(
        Survivor survivor,
        SkinRandomizationMode mode)
    {
        List<SkinPiece> availableSkins = _survivorSkins
            .Where(skin =>
                skin.CharacterId == survivor.Id &&
                !_config.DisabledSurvivorSkins.Contains(skin.Id))
            .ToList();

        if (availableSkins.Count == 0)
        {
            return new List<SkinPiece>();
        }

        if (mode == SkinRandomizationMode.Parts)
        {
            return RandomizeSkinParts(availableSkins);
        }

        return RandomizeSkinOutfit(availableSkins);
    }


    private List<SkinPiece> RandomizeSkinParts(
        List<SkinPiece> skins)
    {
        List<SkinPiece> result = new();

        SkinPart[] parts =
        {
            SkinPart.Head,
            SkinPart.Torso,
            SkinPart.Legs
        };

        foreach (SkinPart part in parts)
        {
            List<SkinPiece> availableParts = skins
                .Where(skin => skin.Part == part)
                .ToList();

            if (availableParts.Count == 0)
            {
                continue;
            }

            SkinPiece selected =
                availableParts[_random.Next(availableParts.Count)];

            result.Add(selected);
        }

        SkinPiece? nonSeparablePiece =
            result.FirstOrDefault(skin => !skin.IsSeparable);

        if (nonSeparablePiece == null)
        {
            return result;
        }

        int outfitId = nonSeparablePiece.OutfitId;

        return skins
            .Where(skin => skin.OutfitId == outfitId)
            .ToList();
    }


    private List<SkinPiece> RandomizeSkinOutfit(
        List<SkinPiece> skins)
    {
        List<int> outfitIds = skins
            .Select(skin => skin.OutfitId)
            .Distinct()
            .ToList();

        if (outfitIds.Count == 0)
        {
            return new List<SkinPiece>();
        }

        int selectedOutfit =
            outfitIds[_random.Next(outfitIds.Count)];

        return skins
            .Where(skin => skin.OutfitId == selectedOutfit)
            .ToList();
    }


    // =========================================================
    // SURVIVOR PERKS
    // =========================================================

    public List<Perk> RandomizeSurvivorPerks(int amount)
    {
        List<Perk> availablePerks = _survivorPerks
            .Where(perk =>
                !_config.DisabledSurvivorPerks.Contains(perk.Id) &&
                (
                    perk.CharacterId == null ||
                    !_config.DisabledSurvivors.Contains(
                        perk.CharacterId.Value
                    )
                ))
            .ToList();

        if (availablePerks.Count < amount)
        {
            throw new InvalidOperationException(
                $"No hay suficientes perks de Survivor disponibles para elegir {amount} perks."
            );
        }

        return availablePerks
            .OrderBy(_ => _random.Next())
            .Take(amount)
            .ToList();
    }


    public Perk RandomizeSurvivorPerk(
        IEnumerable<int> excludedPerkIds)
    {
        List<Perk> availablePerks = _survivorPerks
            .Where(perk =>
                !_config.DisabledSurvivorPerks.Contains(perk.Id) &&
                (
                    perk.CharacterId == null ||
                    !_config.DisabledSurvivors.Contains(
                        perk.CharacterId.Value
                    )
                ) &&
                !excludedPerkIds.Contains(perk.Id))
            .ToList();

        if (availablePerks.Count == 0)
        {
            throw new InvalidOperationException(
                "No hay perks de Survivor disponibles para randomizar."
            );
        }

        return availablePerks[
            _random.Next(availablePerks.Count)
        ];
    }


    // =========================================================
    // SURVIVOR ITEM
    // =========================================================

    public Item RandomizeSurvivorItem()
    {
        List<Item> availableItems = _items
            .Where(item =>
                !_config.DisabledItems.Contains(item.Id))
            .ToList();

        if (availableItems.Count == 0)
        {
            throw new InvalidOperationException(
                "No hay Items disponibles en la configuración."
            );
        }

        return availableItems[
            _random.Next(availableItems.Count)
        ];
    }


    // =========================================================
    // SURVIVOR ADDONS
    // =========================================================

    public List<Addon> RandomizeSurvivorAddons(Item item)
    {
        List<Addon> availableAddons = _addons
            .Where(addon =>
                addon.ItemId == item.Id &&
                !_config.DisabledAddons.Contains(addon.Id))
            .ToList();

        if (availableAddons.Count < 2)
        {
            throw new InvalidOperationException(
                $"El Item '{item.Name}' no tiene al menos 2 addons disponibles."
            );
        }

        return availableAddons
            .OrderBy(_ => _random.Next())
            .Take(2)
            .ToList();
    }


    public Addon RandomizeSurvivorAddon(
        Item item,
        IEnumerable<int> excludedAddonIds)
    {
        List<Addon> availableAddons = _addons
            .Where(addon =>
                addon.ItemId == item.Id &&
                !_config.DisabledAddons.Contains(addon.Id) &&
                !excludedAddonIds.Contains(addon.Id))
            .ToList();

        if (availableAddons.Count == 0)
        {
            throw new InvalidOperationException(
                $"No hay addons de Survivor disponibles para el Item '{item.Name}'."
            );
        }

        return availableAddons[
            _random.Next(availableAddons.Count)
        ];
    }


    // =========================================================
    // SURVIVOR OFFERING
    // =========================================================

    public Offering RandomizeSurvivorOffering()
    {
        List<Offering> availableOfferings = _offerings
            .Where(offering =>
                !_config.DisabledOfferings.Contains(offering.Id))
            .ToList();

        if (availableOfferings.Count == 0)
        {
            throw new InvalidOperationException(
                "No hay Offerings disponibles en la configuración."
            );
        }

        return availableOfferings[
            _random.Next(availableOfferings.Count)
        ];
    }


    // =========================================================
    // SURVIVOR REROLLS
    // =========================================================

    public void RerollSurvivor(RandomizerResult result)
    {
        Survivor survivor = RandomizeSurvivor();

        result.Survivor = survivor;

        result.SurvivorSkin =
            RandomizeSurvivorSkin(
                survivor,
                _config.SurvivorSkinMode
            );
    }


    public void RerollSurvivorSkin(
        RandomizerResult result)
    {
        if (result.Survivor == null)
        {
            throw new InvalidOperationException(
                "No hay un Survivor seleccionado."
            );
        }

        result.SurvivorSkin =
            RandomizeSurvivorSkin(
                result.Survivor,
                _config.SurvivorSkinMode
            );
    }


    public void RerollSurvivorPerk(
        RandomizerResult result,
        int perkIndex)
    {
        if (perkIndex < 0 ||
            perkIndex >= result.SurvivorPerks.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(perkIndex),
                "La posición de la perk no es válida."
            );
        }

        List<int> excludedPerkIds =
            result.SurvivorPerks
                .Select(perk => perk.Id)
                .ToList();

        Perk newPerk =
            RandomizeSurvivorPerk(excludedPerkIds);

        result.SurvivorPerks[perkIndex] = newPerk;
    }


    public void RerollSurvivorPerks(
        RandomizerResult result)
    {
        result.SurvivorPerks =
            RandomizeSurvivorPerks(4);
    }


    public void RerollSurvivorItem(
        RandomizerResult result)
    {
        Item item = RandomizeSurvivorItem();

        result.SurvivorItem = item;

        result.SurvivorAddons =
            RandomizeSurvivorAddons(item);
    }


    public void RerollSurvivorAddon(
        RandomizerResult result,
        int addonIndex)
    {
        if (result.SurvivorItem == null)
        {
            throw new InvalidOperationException(
                "No hay un Item seleccionado."
            );
        }

        if (addonIndex < 0 ||
            addonIndex >= result.SurvivorAddons.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(addonIndex),
                "La posición del addon no es válida."
            );
        }

        List<int> excludedAddonIds =
            result.SurvivorAddons
                .Select(addon => addon.Id)
                .ToList();

        Addon newAddon =
            RandomizeSurvivorAddon(
                result.SurvivorItem,
                excludedAddonIds
            );

        result.SurvivorAddons[addonIndex] = newAddon;
    }


    public void RerollSurvivorOffering(
        RandomizerResult result)
    {
        result.SurvivorOffering =
            RandomizeSurvivorOffering();
    }


    // =========================================================
    // SURVIVOR RESULT
    // =========================================================

    public RandomizerResult RandomizeSurvivorResult()
    {
        Survivor survivor = RandomizeSurvivor();

        RandomizerResult result = new();

        result.Survivor = survivor;

        result.SurvivorSkin =
            RandomizeSurvivorSkin(
                survivor,
                _config.SurvivorSkinMode
            );

        result.SurvivorPerks =
            RandomizeSurvivorPerks(4);

        result.SurvivorItem =
            RandomizeSurvivorItem();

        result.SurvivorAddons =
            RandomizeSurvivorAddons(
                result.SurvivorItem
            );

        result.SurvivorOffering =
            RandomizeSurvivorOffering();

        return result;
    }


    // =========================================================
    // KILLER
    // =========================================================

    public Killer RandomizeKiller()
    {
        List<Killer> availableKillers = _killers
            .Where(killer =>
                !_config.DisabledKillers.Contains(killer.Id))
            .ToList();

        if (availableKillers.Count == 0)
        {
            throw new InvalidOperationException(
                "No hay Killers disponibles en la configuración."
            );
        }

        return availableKillers[
            _random.Next(availableKillers.Count)
        ];
    }


    // =========================================================
    // KILLER SKINS
    // =========================================================

    private List<SkinPiece> RandomizeKillerSkinParts(
        List<SkinPiece> skins)
    {
        List<SkinPiece> result = new();

        SkinPart[] parts =
        {
            SkinPart.Head,
            SkinPart.Torso,
            SkinPart.Weapon
        };

        foreach (SkinPart part in parts)
        {
            List<SkinPiece> availableParts = skins
                .Where(skin => skin.Part == part)
                .ToList();

            if (availableParts.Count == 0)
            {
                continue;
            }

            SkinPiece selected =
                availableParts[_random.Next(availableParts.Count)];

            result.Add(selected);
        }

        SkinPiece? nonSeparablePiece =
            result.FirstOrDefault(skin => !skin.IsSeparable);

        if (nonSeparablePiece == null)
        {
            return result;
        }

        int outfitId = nonSeparablePiece.OutfitId;

        return skins
            .Where(skin => skin.OutfitId == outfitId)
            .ToList();
    }


    public List<SkinPiece> RandomizeKillerSkin(
        Killer killer,
        SkinRandomizationMode mode)
    {
        List<SkinPiece> availableSkins = _killerSkins
            .Where(skin =>
                skin.CharacterId == killer.Id &&
                !_config.DisabledKillerSkins.Contains(skin.Id))
            .ToList();

        if (availableSkins.Count == 0)
        {
            return new List<SkinPiece>();
        }

        if (mode == SkinRandomizationMode.Parts)
        {
            return RandomizeKillerSkinParts(availableSkins);
        }

        return RandomizeSkinOutfit(availableSkins);
    }


    // =========================================================
    // KILLER PERKS
    // =========================================================

    public List<Perk> RandomizeKillerPerks(int amount)
    {
        List<Perk> availablePerks = _killerPerks
            .Where(perk =>
                !_config.DisabledKillerPerks.Contains(perk.Id) &&
                (
                    perk.CharacterId == null ||
                    !_config.DisabledKillers.Contains(
                        perk.CharacterId.Value
                    )
                ))
            .ToList();

        if (availablePerks.Count < amount)
        {
            throw new InvalidOperationException(
                $"No hay suficientes perks de Killer disponibles para elegir {amount} perks."
            );
        }

        return availablePerks
            .OrderBy(_ => _random.Next())
            .Take(amount)
            .ToList();
    }


    public Perk RandomizeKillerPerk(
        IEnumerable<int> excludedPerkIds)
    {
        List<Perk> availablePerks = _killerPerks
            .Where(perk =>
                !_config.DisabledKillerPerks.Contains(perk.Id) &&
                (
                    perk.CharacterId == null ||
                    !_config.DisabledKillers.Contains(
                        perk.CharacterId.Value
                    )
                ) &&
                !excludedPerkIds.Contains(perk.Id))
            .ToList();

        if (availablePerks.Count == 0)
        {
            throw new InvalidOperationException(
                "No hay perks de Killer disponibles para randomizar."
            );
        }

        return availablePerks[
            _random.Next(availablePerks.Count)
        ];
    }


    // =========================================================
    // KILLER ADDONS
    // =========================================================

    public List<Addon> RandomizeKillerAddons(
        Killer killer)
    {
        List<Addon> availableAddons = _addons
            .Where(addon =>
                addon.CharacterId == killer.Id &&
                !_config.DisabledAddons.Contains(addon.Id))
            .ToList();

        if (availableAddons.Count < 2)
        {
            throw new InvalidOperationException(
                $"El Killer '{killer.Name}' no tiene al menos 2 addons disponibles."
            );
        }

        return availableAddons
            .OrderBy(_ => _random.Next())
            .Take(2)
            .ToList();
    }


    public Addon RandomizeKillerAddon(
        Killer killer,
        IEnumerable<int> excludedAddonIds)
    {
        List<Addon> availableAddons = _addons
            .Where(addon =>
                addon.CharacterId == killer.Id &&
                !_config.DisabledAddons.Contains(addon.Id) &&
                !excludedAddonIds.Contains(addon.Id))
            .ToList();

        if (availableAddons.Count == 0)
        {
            throw new InvalidOperationException(
                $"No hay addons de Killer disponibles para '{killer.Name}'."
            );
        }

        return availableAddons[
            _random.Next(availableAddons.Count)
        ];
    }


    // =========================================================
    // KILLER OFFERING
    // =========================================================

    public Offering RandomizeKillerOffering()
    {
        List<Offering> availableOfferings = _offerings
            .Where(offering =>
                !_config.DisabledOfferings.Contains(offering.Id))
            .ToList();

        if (availableOfferings.Count == 0)
        {
            throw new InvalidOperationException(
                "No hay Offerings disponibles en la configuración."
            );
        }

        return availableOfferings[
            _random.Next(availableOfferings.Count)
        ];
    }


    // =========================================================
    // KILLER RESULT
    // =========================================================

    public RandomizerResult RandomizeKillerResult()
    {
        Killer killer = RandomizeKiller();

        RandomizerResult result = new();

        result.Killer = killer;

        result.KillerSkin =
            RandomizeKillerSkin(
                killer,
                _config.KillerSkinMode
            );

        result.KillerPerks =
            RandomizeKillerPerks(4);

        result.KillerAddons =
            RandomizeKillerAddons(killer);

        result.KillerOffering =
            RandomizeKillerOffering();

        return result;
    }


    // =========================================================
    // KILLER REROLLS
    // =========================================================

    public void RerollKiller(
        RandomizerResult result)
    {
        Killer killer = RandomizeKiller();

        result.Killer = killer;

        result.KillerSkin =
            RandomizeKillerSkin(
                killer,
                _config.KillerSkinMode
            );

        result.KillerAddons =
            RandomizeKillerAddons(killer);
    }


    public void RerollKillerPerk(
        RandomizerResult result,
        int perkIndex)
    {
        if (perkIndex < 0 ||
            perkIndex >= result.KillerPerks.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(perkIndex),
                "La posición de la perk no es válida."
            );
        }

        List<int> excludedPerkIds =
            result.KillerPerks
                .Select(perk => perk.Id)
                .ToList();

        Perk newPerk =
            RandomizeKillerPerk(excludedPerkIds);

        result.KillerPerks[perkIndex] = newPerk;
    }


    public void RerollKillerPerks(
        RandomizerResult result)
    {
        result.KillerPerks =
            RandomizeKillerPerks(4);
    }


    public void RerollKillerAddon(
        RandomizerResult result,
        int addonIndex)
    {
        if (result.Killer == null)
        {
            throw new InvalidOperationException(
                "No hay un Killer seleccionado."
            );
        }

        if (addonIndex < 0 ||
            addonIndex >= result.KillerAddons.Count)
        {
            throw new ArgumentOutOfRangeException(
                nameof(addonIndex),
                "La posición del addon no es válida."
            );
        }

        List<int> excludedAddonIds =
            result.KillerAddons
                .Select(addon => addon.Id)
                .ToList();

        Addon newAddon =
            RandomizeKillerAddon(
                result.Killer,
                excludedAddonIds
            );

        result.KillerAddons[addonIndex] = newAddon;
    }


    public void RerollKillerOffering(
        RandomizerResult result)
    {
        result.KillerOffering =
            RandomizeKillerOffering();
    }


    public void RerollKillerSkin(
        RandomizerResult result)
    {
        if (result.Killer == null)
        {
            throw new InvalidOperationException(
                "No hay un Killer seleccionado."
            );
        }

        result.KillerSkin =
            RandomizeKillerSkin(
                result.Killer,
                _config.KillerSkinMode
            );
    }
}