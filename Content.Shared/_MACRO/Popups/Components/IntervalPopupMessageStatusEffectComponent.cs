using Robust.Shared.GameStates;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Shared._MACRO.Popups.Components;

/// <summary>
///     A status effect that will regularly create a popup message on the entity on a given interval.
/// </summary>
/// <remarks>
///     This is very similar to the "PopupMessage" entity effect in metabolisms, but rather than
///     being a chance per metabolism tick, this just shows random messages on a given interval -
///     making it more consistent.
/// </remarks>
[RegisterComponent]
[NetworkedComponent, AutoGenerateComponentState, AutoGenerateComponentPause]
public sealed partial class IntervalPopupMessageStatusEffectComponent : PopupMessageStatusEffectComponent
{
    /// <summary>
    ///     The minimum and maximum time interval that popup messages will be displayed.
    /// </summary>
    [DataField]
    public (TimeSpan Min, TimeSpan Max) Interval = (TimeSpan.FromSeconds(30.0f), TimeSpan.FromSeconds(60.0f));

    /// <summary>
    ///     The next time we should display a popup message.
    /// </summary>
    [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
    [AutoNetworkedField, AutoPausedField]
    public TimeSpan NextPopupTime = TimeSpan.Zero;
}
