using DBDRandomizer.Models;

namespace DBDRandomizer.Randomizer;

public class RandomizerEngine
{
    private readonly Random _random = new();

    private bool ShouldBeEmpty(int chance)
    {
        return _random.Next(100) < chance;
    }

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
    private readonly List<TwoV8Class> _twoV8Classes;
    private readonly List<TwoV8Skill> _twoV8Skills;
    private readonly TwoV8Config _twoV8Config;

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
        RandomizerConfig config,
        List<TwoV8Class> twoV8Classes,
        List<TwoV8Skill> twoV8Skills,
        TwoV8Config twoV8Config)
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

        _twoV8Classes = twoV8Classes;
        _twoV8Skills = twoV8Skills;
        _twoV8Config = twoV8Config;
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

    public List<Perk?> RandomizeSurvivorPerks(int amount)
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

        List<Perk?> result = availablePerks
            .OrderBy(_ => _random.Next())
            .Take(amount)
            .Cast<Perk?>()
            .ToList();

        for (int i = 0; i < result.Count; i++)
        {
            if (ShouldBeEmpty(_config.PerkEmptyChance))
            {
                result[i] = null;
            }
        }

        return result;
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
            .Where(item =>
                item.AddonGroupId == null ||
                _addons.Count(addon =>
                    addon.AddonGroupId == item.AddonGroupId &&
                    addon.CharacterId == null &&
                    !_config.DisabledAddons.Contains(addon.Id)) >= 2)
            .ToList();

        if (availableItems.Count == 0)
        {
            throw new InvalidOperationException(
                "No hay Items disponibles."
            );
        }

        return availableItems[
            _random.Next(availableItems.Count)
        ];
    }


    // =========================================================
    // SURVIVOR ADDONS
    // =========================================================

    public List<Addon?> RandomizeSurvivorAddons(Item item)
    {
        // El Item no tiene addons.
        if (item.AddonGroupId == null)
        {
            return new List<Addon?>
            {
                null,
                null
            };
        }

        List<Addon> availableAddons = _addons
            .Where(addon =>
                addon.AddonGroupId == item.AddonGroupId &&
                addon.CharacterId == null &&
                !_config.DisabledAddons.Contains(addon.Id))
            .ToList();

        if (availableAddons.Count < 2)
        {
            throw new InvalidOperationException(
                $"El grupo de addons del Item '{item.Name}' " +
                "no tiene al menos 2 addons disponibles."
            );
        }

        List<Addon?> result = availableAddons
            .OrderBy(_ => _random.Next())
            .Take(2)
            .Cast<Addon?>()
            .ToList();

        for (int i = 0; i < result.Count; i++)
        {
            if (ShouldBeEmpty(_config.AddonEmptyChance))
            {
                result[i] = null;
            }
        }

        return result;
    }


    public Addon RandomizeSurvivorAddon(
        Item item,
        IEnumerable<int> excludedAddonIds)
    {
        if (item.AddonGroupId == null)
        {
            throw new InvalidOperationException(
                $"El Item '{item.Name}' no tiene addons."
            );
        }

        List<Addon> availableAddons = _addons
            .Where(addon =>
                addon.AddonGroupId == item.AddonGroupId &&
                addon.CharacterId == null &&
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

    public Offering? RandomizeSurvivorOffering()
    {
        List<Offering> availableOfferings = _offerings
            .Where(offering =>
                !_config.DisabledOfferings.Contains(offering.Id) &&
                (
                    offering.Role == "Survivor" ||
                    offering.Role == "Both"
                ))
            .ToList();

        if (availableOfferings.Count == 0)
        {
            throw new InvalidOperationException(
                "No hay Offerings de Survivor disponibles en la configuración."
            );
        }

        if (ShouldBeEmpty(_config.OfferingEmptyChance))
        {
            return null;
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

        if (ShouldBeEmpty(_config.PerkEmptyChance))
        {
            result.SurvivorPerks[perkIndex] = null;
            return;
        }

        List<int> excludedPerkIds =
            result.SurvivorPerks
                .Where(perk => perk != null)
                .Select(perk => perk!.Id)
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


    public void RerollSurvivorItem(RandomizerResult result)
    {
        if (ShouldBeEmpty(_config.ItemEmptyChance))
        {
            result.SurvivorItem = null;
            result.SurvivorAddons = new List<Addon?> { null, null };
            return;
        }

        Item item = RandomizeSurvivorItem();

        result.SurvivorItem = item;

        result.SurvivorAddons = RandomizeSurvivorAddons(item);
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

        if (ShouldBeEmpty(_config.AddonEmptyChance))
        {
            result.SurvivorAddons[addonIndex] = null;
            return;
        }

        List<int> excludedAddonIds =
            result.SurvivorAddons
                .Where(addon => addon != null)
                .Select(addon => addon!.Id)
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

        if (ShouldBeEmpty(_config.ItemEmptyChance))
        {
            result.SurvivorItem = null;
            result.SurvivorAddons = new List<Addon?> { null, null };
        }
        else
        {
            result.SurvivorItem =
                RandomizeSurvivorItem();

            result.SurvivorAddons =
                RandomizeSurvivorAddons(
                    result.SurvivorItem
                );
        }

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
        .Where(killer =>
            _addons.Count(addon =>
                addon.CharacterId == killer.Id &&
                !_config.DisabledAddons.Contains(addon.Id)) >= 2)
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

    public List<Perk?> RandomizeKillerPerks(int amount)
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

        List<Perk?> result = availablePerks
            .OrderBy(_ => _random.Next())
            .Take(amount)
            .Cast<Perk?>()
            .ToList();

        for (int i = 0; i < result.Count; i++)
        {
            if (ShouldBeEmpty(_config.PerkEmptyChance))
            {
                result[i] = null;
            }
        }

        return result;
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

    public List<Addon?> RandomizeKillerAddons(Killer killer)
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

        List<Addon?> result = availableAddons
            .OrderBy(_ => _random.Next())
            .Take(2)
            .Cast<Addon?>()
            .ToList();

        for (int i = 0; i < result.Count; i++)
        {
            if (ShouldBeEmpty(_config.AddonEmptyChance))
            {
                result[i] = null;
            }
        }

        return result;
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

    public Offering? RandomizeKillerOffering()
    {
        List<Offering> availableOfferings = _offerings
            .Where(offering =>
                !_config.DisabledOfferings.Contains(offering.Id) &&
                (
                    offering.Role == "Killer" ||
                    offering.Role == "Both"
                ))
            .ToList();

        if (availableOfferings.Count == 0)
        {
            throw new InvalidOperationException(
                "No hay Offerings de Killer disponibles en la configuración."
            );
        }

        if (ShouldBeEmpty(_config.OfferingEmptyChance))
        {
            return null;
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

        if (ShouldBeEmpty(_config.PerkEmptyChance))
        {
            result.KillerPerks[perkIndex] = null;
            return;
        }

        List<int> excludedPerkIds =
            result.KillerPerks
                .Where(perk => perk != null)
                .Select(perk => perk!.Id)
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

        if (ShouldBeEmpty(_config.AddonEmptyChance))
        {
            result.KillerAddons[addonIndex] = null;
            return;
        }

        List<int> excludedAddonIds =
            result.KillerAddons
                .Where(addon => addon != null)
                .Select(addon => addon!.Id)
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

    // =========================================================
    // 2v8
    // =========================================================

    public Survivor RandomizeTwoV8Survivor()
    {
        List<Survivor> availableSurvivors = _survivors
            .Where(survivor =>
                !_config.DisabledSurvivors.Contains(survivor.Id))
            .ToList();

        if (availableSurvivors.Count == 0)
        {
            throw new InvalidOperationException(
                "No hay Survivors disponibles para 2v8."
            );
        }

        return availableSurvivors[
            _random.Next(availableSurvivors.Count)
        ];
    }

    public TwoV8Class RandomizeTwoV8SurvivorClass()
    {
        List<TwoV8Class> classes = _twoV8Classes
            .Where(c => c.Role == "Survivor")
            .ToList();

        if (classes.Count == 0)
        {
            throw new InvalidOperationException(
                "No hay clases de Survivor disponibles para 2v8."
            );
        }

        return classes[
            _random.Next(classes.Count)
        ];
    }

    public List<TwoV8Skill> GetTwoV8Skills(
        TwoV8Class selectedClass)
    {
        return _twoV8Skills
            .Where(skill => skill.ClassId == selectedClass.Id)
            .OrderBy(skill => skill.Slot)
            .ToList();
    }

    public TwoV8Result RandomizeTwoV8SurvivorResult()
    {
        Survivor survivor = RandomizeTwoV8Survivor();

        TwoV8Class selectedClass =
            RandomizeTwoV8SurvivorClass();

        List<TwoV8Skill> skills =
            GetTwoV8Skills(selectedClass);

        if (skills.Count != 3)
        {
            throw new InvalidOperationException(
                $"La clase '{selectedClass.Name}' debe tener exactamente 3 habilidades."
            );
        }

        return new TwoV8Result
        {
            Survivor = survivor,
            Class = selectedClass,
            Skills = skills
        };
    }

    public Killer RandomizeTwoV8Killer()
    {
        List<Killer> availableKillers = _killers
            .Where(killer =>
                _twoV8Config.Killers.Contains(killer.Id) &&
                !_config.DisabledKillers.Contains(killer.Id))
            .ToList();

        if (availableKillers.Count == 0)
        {
            throw new InvalidOperationException(
                "No hay Killers disponibles para 2v8."
            );
        }

        return availableKillers[
            _random.Next(availableKillers.Count)
        ];
    }

    public TwoV8Skill GetTwoV8KillerInnateSkill(
        Killer killer)
    {
        TwoV8Skill? skill = _twoV8Skills
            .FirstOrDefault(skill =>
                skill.Role == "Killer" &&
                skill.Type == "Innate" &&
                skill.CharacterId == killer.Id);

        if (skill == null)
        {
            throw new InvalidOperationException(
                $"No se encontró la habilidad innata de 2v8 para el Killer '{killer.Name}'."
            );
        }

        return skill;
    }

    public TwoV8Result RandomizeTwoV8KillerResult()
    {
        Killer killer = RandomizeTwoV8Killer();

        TwoV8Class selectedClass = _twoV8Classes
            .Where(c => c.Role == "Killer")
            .OrderBy(_ => _random.Next())
            .FirstOrDefault()
            ?? throw new InvalidOperationException(
                "No hay clases de Killer disponibles para 2v8."
            );

        List<TwoV8Skill> classSkills =
            GetTwoV8Skills(selectedClass);

        if (classSkills.Count != 2)
        {
            throw new InvalidOperationException(
                $"La clase '{selectedClass.Name}' debe tener exactamente 2 habilidades de clase."
            );
        }

        TwoV8Skill innateSkill =
            GetTwoV8KillerInnateSkill(killer);

        return new TwoV8Result
        {
            Killer = killer,
            Class = selectedClass,
            Skills = classSkills,
            KillerInnateSkill = innateSkill
        };
    }

    public void RerollTwoV8Survivor(TwoV8Result result)
    {
        result.Survivor = RandomizeTwoV8Survivor();
    }

    public void RerollTwoV8SurvivorClass(TwoV8Result result)
    {
        result.Class = RandomizeTwoV8SurvivorClass();

        List<TwoV8Skill> skills =
            GetTwoV8Skills(result.Class);

        if (skills.Count != 3)
        {
            throw new InvalidOperationException(
                $"La clase '{result.Class.Name}' debe tener exactamente 3 habilidades."
            );
        }

        result.Skills = skills;
    }

    public void RerollTwoV8Skill(TwoV8Result result,int index)
    {
        if (result.Class == null)
        {
            return;
        }

        List<TwoV8Skill> availableSkills =
            GetTwoV8Skills(result.Class);

        if (index < 0 || index >= availableSkills.Count)
        {
            return;
        }

        result.Skills[index] =
            availableSkills[index];
    }

    public void RerollTwoV8Killer(TwoV8Result result)
    {
        Killer killer = RandomizeTwoV8Killer();

        result.Killer = killer;

        result.KillerInnateSkill =
            GetTwoV8KillerInnateSkill(killer);
    }

    public void RerollTwoV8KillerClass(TwoV8Result result)
    {
        TwoV8Class selectedClass = _twoV8Classes
            .Where(c => c.Role == "Killer")
            .OrderBy(_ => _random.Next())
            .FirstOrDefault()
            ?? throw new InvalidOperationException(
                "No hay clases de Killer disponibles para 2v8."
            );

        List<TwoV8Skill> classSkills =
            GetTwoV8Skills(selectedClass);

        if (classSkills.Count != 2)
        {
            throw new InvalidOperationException(
                $"La clase '{selectedClass.Name}' debe tener exactamente 2 habilidades de clase."
            );
        }

        result.Class = selectedClass;
        result.Skills = classSkills;
    }

}

