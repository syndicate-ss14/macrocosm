using Robust.Shared.GameStates;

namespace Content.Shared._Box.Traits;

/// <summary>
/// Set an affix to all instances of the players original species
/// i.e. "Vulpkanin" > "Synthetic Vulpkanin" or "Vox" > "Vox Automata"
/// </summary>
/// <remark>
/// based heavily on the Cosmatic Drift synth trait
/// Box - Added support for custom strings and generalizes it to any affix
/// </remark>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class SpeciesAffixComponent : Component
{

    /// <summary>
    /// The Loc String to pull when the entity is Examined
    /// The string must always take a "species" arguement
    /// </summary>
    [DataField, AutoNetworkedField]
    public string ExamineAffix { get; set; } = "strange-affix-examine";

    /// <summary>
    /// The Loc String to pull in all other circumstances
    /// The string must always take a "species" arguement
    /// </summary>
    [DataField, AutoNetworkedField]
    public string Affix { get; set; } = "strange-species-affix";

}
