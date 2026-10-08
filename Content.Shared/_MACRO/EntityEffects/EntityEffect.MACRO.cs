using Content.Shared.Localizations;
using Robust.Shared.Prototypes;
using System.Linq;

namespace Content.Shared.EntityEffects;

public abstract partial class EntityEffect
{
    /// <summary>
    ///     This text provided by a condition acts as a flag to indicate that a metabolism effect
    ///     should be hidden from the guidebook entirely.
    /// </summary>
    // TODO: this is a terrible idea but it's the least merge conflicty way i could think of doing it.
    // maybe come up with a better way
    public const string HideEffectTag = "!HIDEME!";

    /// <summary>
    ///     Get a localized string representation of this effect's conditions.
    /// </summary>
    /// <param name="count">The number of valid conditions.</param>
    /// <param name="showEntry">Whether or not this effect should be shown in the guidebook.</param>
    public string GetConditions(IPrototypeManager prototype, out int count, out bool showEntry)
    {
        showEntry = true;
        var conditions = Conditions?
            .Select(x => x.EntityConditionGuidebookText(prototype))
            .Where(x => x != string.Empty)
            .ToList() ?? new();

        count = conditions.Count;

        // Hide this entry if one of the conditions indicates it should be hidden.
        if (conditions.Contains(HideEffectTag))
        {
            showEntry = false;
            return string.Empty;
        }

        return ContentLocalizationManager.FormatList(conditions);
    }
}
