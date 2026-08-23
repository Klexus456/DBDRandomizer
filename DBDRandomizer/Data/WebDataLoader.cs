using System.Text.Json;
using System.Text.Json.Serialization;
using DBDRandomizer.Models;

namespace DBDRandomizer.Data;

public class WebDataLoader
{
    private readonly HttpClient _httpClient;

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public WebDataLoader(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private async Task<T> LoadJson<T>(string fileName)
    {
        string json = await _httpClient.GetStringAsync(
            $"data/{fileName}"
        );

        return JsonSerializer.Deserialize<T>(
            json,
            _options
        ) ?? throw new Exception(
            $"No se pudo cargar {fileName}"
        );
    }

    public Task<RandomizerConfig> LoadConfig()
    {
        return LoadJson<RandomizerConfig>("config.json");
    }

    public Task<List<Survivor>> LoadSurvivors()
    {
        return LoadJson<List<Survivor>>("survivors.json");
    }

    public Task<List<Killer>> LoadKillers()
    {
        return LoadJson<List<Killer>>("killers.json");
    }

    public Task<List<Perk>> LoadSurvivorPerks()
    {
        return LoadJson<List<Perk>>("perks_survivor.json");
    }

    public Task<List<Perk>> LoadKillerPerks()
    {
        return LoadJson<List<Perk>>("perks_killer.json");
    }

    public Task<List<Addon>> LoadAddons()
    {
        return LoadJson<List<Addon>>("addons.json");
    }

    public Task<List<Offering>> LoadOfferings()
    {
        return LoadJson<List<Offering>>("offerings.json");
    }

    public Task<List<SkinPiece>> LoadSurvivorSkins()
    {
        return LoadJson<List<SkinPiece>>("survivor_skins.json");
    }

    public Task<List<SkinPiece>> LoadKillerSkins()
    {
        return LoadJson<List<SkinPiece>>("killer_skins.json");
    }

    public Task<List<Item>> LoadItems()
    {
        return LoadJson<List<Item>>("items.json");
    }
}