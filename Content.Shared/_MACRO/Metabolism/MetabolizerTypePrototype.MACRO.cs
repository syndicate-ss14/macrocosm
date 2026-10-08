using Content.Shared.EntityConditions.Conditions.Body;

namespace Content.Shared.Metabolism;

public sealed partial class MetabolizerTypePrototype
{
    /// <summary>
    ///     Whether or not this metabolizer type will show up in the guidebook
    ///     when included in a <seealso cref="MetabolizerTypeCondition"/>.
    /// </summary>
    /// <remarks>
    ///     Effects with metabolizer requirements that are all hidden from the guidebook will hide the effect
    ///     entirely if the requirement is not inverted.
    ///     If you hide metabolizers from the guidebook that are actually obtainable in-game, it may appear
    ///     to players that reagents have "undocumented" effects that do not appear in the guidebook, which
    ///     some may argue defeats the point of the guidebook!
    ///
    ///     This field is designed for metabolizers that "don't exist" at all - such as metabolizers used
    ///     exclusively by species that are not enabled.
    /// </remarks>
    [DataField]
    public bool ShowInGuidebook = true;
}
