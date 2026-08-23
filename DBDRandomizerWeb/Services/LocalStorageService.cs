using System.Text.Json;
using DBDRandomizer.Models;
using Microsoft.JSInterop;

namespace DBDRandomizerWeb.Services;

public class LocalStorageService
{
    private const string ConfigKey = "dbd-randomizer-config";

    private readonly IJSRuntime _jsRuntime;

    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SaveConfig(RandomizerConfig config)
    {
        string json = JsonSerializer.Serialize(config, _options);

        await _jsRuntime.InvokeVoidAsync(
            "localStorageService.save",
            ConfigKey,
            json
        );
    }

    public async Task<RandomizerConfig?> LoadConfig()
    {
        string? json = await _jsRuntime.InvokeAsync<string?>(
            "localStorageService.load",
            ConfigKey
        );

        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonSerializer.Deserialize<RandomizerConfig>(
            json,
            _options
        );
    }

    public async Task RemoveConfig()
    {
        await _jsRuntime.InvokeVoidAsync(
            "localStorageService.remove",
            ConfigKey
        );
    }
}