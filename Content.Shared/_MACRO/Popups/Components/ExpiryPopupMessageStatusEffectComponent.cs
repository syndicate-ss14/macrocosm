using Robust.Shared.GameStates;

namespace Content.Shared._MACRO.Popups.Components;

/// <summary>
///     A status effect that will create a popup message on the entity upon the status effect expiring.
/// </summary>
/// <remarks>
///     This is very similar to the "PopupMessage" entity effect in metabolisms, but rather than
///     being a chance per metabolism tick, this just shows a random message on expiry -
///     making it more consistent.
/// </remarks>
[RegisterComponent]
[NetworkedComponent]
public sealed partial class ExpiryPopupMessageStatusEffectComponent : PopupMessageStatusEffectComponent
{ }
