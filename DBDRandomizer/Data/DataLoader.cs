using System.Text.Json;
using DBDRandomizer.Models;
using System.Text.Json.Serialization;

namespace DBDRandomizer.Data;

public class DataLoader
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    private T LoadJson<T>(string fileName)
    {
        string path = Path.Combine("Data", fileName);
        string json = File.ReadAllText(path);

        return JsonSerializer.Deserialize<T>(json, _options)
               ?? throw new Exception($"No se pudo cargar {fileName}");
    }

    public RandomizerConfig LoadConfig()
    {
        return LoadJson<RandomizerConfig>("config.json");
    }

    public List<Survivor> LoadSurvivors()
    {
        return LoadJson<List<Survivor>>("survivors.json");
    }

    public List<Killer> LoadKillers()
    {
        return LoadJson<List<Killer>>("killers.json");
    }

    public List<Perk> LoadSurvivorPerks()
    {
        return LoadJson<List<Perk>>("perks_survivor.json");
    }

    public List<Perk> LoadKillerPerks()
    {
        return LoadJson<List<Perk>>("perks_killer.json");
    }

    public List<Addon> LoadAddons()
    {
        return LoadJson<List<Addon>>("addons.json");
    }

    public List<Offering> LoadOfferings()
    {
        return LoadJson<List<Offering>>("offerings.json");
    }

    public List<SkinPiece> LoadSurvivorSkins()
    {
        return LoadJson<List<SkinPiece>>("survivor_skins.json");
    }

    public List<SkinPiece> LoadKillerSkins()
    {
        return LoadJson<List<SkinPiece>>("killer_skins.json");
    }

    public List<Item> LoadItems()
    {
        return LoadJson<List<Item>>("items.json");
    }
}