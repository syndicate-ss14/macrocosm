using Robust.Shared.GameStates;

namespace Content.Shared._MACRO.Traits;

/// <summary>
/// Set an affix to all instances of the players original species
/// i.e. "Vulpkanin" > "Synthetic Vulpkanin" or "Vox" > "Vox Automata"
/// </summary>
/// <remark>
/// This feature is originally from Box Station PR https://github.com/ss14-boxstation/ss14-boxstation/pull/342
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
