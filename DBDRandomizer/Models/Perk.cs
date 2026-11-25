using System.Text.Json.Serialization;

namespace DBDRandomizer.Models;

public class Perk
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public int? CharacterId { get; set; }
    public string Description { get; set; } = "";
    public string Image { get; set; } = "";

    // Solo se completa en perks GENERALES que reemplazan a una perk
    // licenciada cuya licencia terminó (ej. "Que Sigan Esperando" reemplaza
    // a "Lo Mejor para el Final"). Apunta al Id de la perk licenciada.
    // Ambas son excluyentes: no se puede tener las dos habilitadas.
    // Si es null no se escribe en el JSON (así la página de Diagnóstico no
    // agrega "LicensedPerkId": null a todas las perks al guardar).
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? LicensedPerkId { get; set; }
}