using Robust.Shared.GameStates;

namespace Content.Shared._MACRO.Nutrition.Components;

/// <summary>
/// This component is used to slow down the time to eat food by a specific multiplier
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class EatTimeModifierComponent : Component
{
    /// <summary>
    /// The amount you would like to multiply the delay on the eating doAfter
    /// </summary>
    [DataField]
    public float Multiplier = 1f;
}
