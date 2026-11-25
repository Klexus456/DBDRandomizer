using Microsoft.AspNetCore.Components;

namespace DBDRandomizerWeb.Components;

/// <summary>
/// Convierte los marcadores de color de las descripciones
/// ({YELLOW}, {BLUE}, {RESET}, etc.) en HTML.
///
/// Es la ÚNICA fuente de verdad de los colores: Home (a través de
/// ItemTooltip), Settings y TwoV8 usan esta clase. Si se agrega un
/// color nuevo, se agrega acá y listo.
/// </summary>
public static class DescriptionFormatter
{
    private static readonly Dictionary<string, string> Colors = new()
    {
        ["{YELLOW}"] = "#e8c252",
        ["{ORANGE}"] = "#ff8800",
        ["{BLUE}"] = "#4da997",
        ["{GREEN}"] = "#199b1e",
        ["{CYAN}"] = "#2f9cd6",
        ["{PURPLE}"] = "#ac3ee3",
        ["{CITATION}"] = "#e7cda2"
    };

    public static MarkupString Render(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            return new MarkupString("");
        }

        string result = description;

        foreach (var color in Colors)
        {
            result = result.Replace(
                color.Key,
                $"<span style=\"color:{color.Value};\">"
            );
        }

        result = result.Replace("{RESET}", "</span>");

        return new MarkupString(result);
    }
}
