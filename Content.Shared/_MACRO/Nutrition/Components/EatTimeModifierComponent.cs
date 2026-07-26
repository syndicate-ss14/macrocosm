using Content.Shared.Whitelist;
using Robust.Shared.GameStates;

namespace Content.Shared._MACRO.Nutrition.Components;

/// <summary>
/// This component is used to modify the time to eat food by a specific modifier.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class EatTimeModifierComponent : Component
{
    /// <summary>
    /// The amount you would like to multiply the delay on the eating doAfter
    /// </summary>
    [DataField]
    public float Modifier = 1f;

    /// <summary>
    /// If not null, the target must succeed the whitelist.
    /// </summary>
    [DataField]
    public EntityWhitelist? TargetWhitelist { get; set; }

    /// <summary>
    /// If not null, the target must not pass the blacklist.
    /// </summary>
    [DataField]
    public EntityWhitelist? TargetBlacklist { get; set; }
}
