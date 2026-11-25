using DBDRandomizer.Models;

namespace DBDRandomizer.Randomizer;

/// <summary>
/// Maneja las perks licenciadas que pasaron a tener una versión general
/// (ej. las de Myers, o las de Chucky cuando se agreguen).
///
/// Cada perk general tiene LicensedPerkId apuntando a su perk licenciada.
/// Son excluyentes: si tenés una habilitada, la otra se deshabilita.
/// Agregar un caso nuevo solo requiere completar LicensedPerkId en el JSON.
/// </summary>
public static class PerkLinks
{
    /// <summary>
    /// Ids de las perks excluyentes con perkId:
    /// - si perkId es general, devuelve su perk licenciada;
    /// - si perkId es licenciada, devuelve sus perks generales.
    /// </summary>
    public static List<int> GetCounterparts(List<Perk> perks, int perkId)
    {
        Perk? perk = perks.FirstOrDefault(p => p.Id == perkId);

        if (perk == null)
        {
            return new List<int>();
        }

        if (perk.LicensedPerkId.HasValue)
        {
            int licensedId = perk.LicensedPerkId.Value;

            return perks.Any(p => p.Id == licensedId)
                ? new List<int> { licensedId }
                : new List<int>();
        }

        return perks
            .Where(p => p.LicensedPerkId == perkId)
            .Select(p => p.Id)
            .ToList();
    }

    /// <summary>
    /// true si alguna perk excluyente con perkId está habilitada.
    /// </summary>
    public static bool HasEnabledCounterpart(
        List<Perk> perks,
        List<int> disabledPerkIds,
        int perkId)
    {
        return GetCounterparts(perks, perkId)
            .Any(id => !disabledPerkIds.Contains(id));
    }

    /// <summary>
    /// Habilita o deshabilita una perk. Al HABILITAR una perk se
    /// deshabilitan sus perks excluyentes. Al deshabilitarla no se toca
    /// la otra (que el usuario saque una perk no implica que tenga la otra).
    /// </summary>
    public static void Toggle(
        List<Perk> perks,
        List<int> disabledPerkIds,
        int perkId)
    {
        if (disabledPerkIds.Contains(perkId))
        {
            disabledPerkIds.Remove(perkId);

            foreach (int otherId in GetCounterparts(perks, perkId))
            {
                if (!disabledPerkIds.Contains(otherId))
                {
                    disabledPerkIds.Add(otherId);
                }
            }
        }
        else
        {
            disabledPerkIds.Add(perkId);
        }
    }

    /// <summary>
    /// Corrige configuraciones donde una perk licenciada y su general
    /// están habilitadas a la vez (config por defecto, "habilitar todas",
    /// o perks generales nuevas agregadas a los datos).
    /// En ese caso se mantiene la licenciada y se deshabilita la general.
    /// Si el usuario quiere la general, la habilita en Settings y eso
    /// deshabilita la licenciada. Devuelve true si modificó la lista.
    /// </summary>
    public static bool Normalize(
        List<Perk> perks,
        List<int> disabledPerkIds)
    {
        bool changed = false;

        foreach (Perk general in perks.Where(p => p.LicensedPerkId.HasValue))
        {
            int licensedId = general.LicensedPerkId!.Value;

            bool licensedExists = perks.Any(p => p.Id == licensedId);

            if (licensedExists &&
                !disabledPerkIds.Contains(licensedId) &&
                !disabledPerkIds.Contains(general.Id))
            {
                disabledPerkIds.Add(general.Id);
                changed = true;
            }
        }

        return changed;
    }
}
