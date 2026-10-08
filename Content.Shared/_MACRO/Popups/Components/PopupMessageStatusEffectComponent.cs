using Content.Shared.EntityEffects.Effects.Transform;
using Content.Shared.Popups;

namespace Content.Shared._MACRO.Popups.Components;

/// <summary>
///     Abstract class for status effects that show popup messages.
/// </summary>
/// <remarks>
///     This is very similar to the "PopupMessage" entity effect in metabolisms.
/// </remarks>
public abstract partial class PopupMessageStatusEffectComponent : Component
{
    /// <summary>
    /// Array of messages that can popup.
    /// Only one is chosen when the effect is applied.
    /// </summary>
    [DataField(required: true)]
    public string[] Messages = default!;

    /// <summary>
    /// Whether to just the entity we're affecting, or everyone around them.
    /// </summary>
    [DataField]
    public PopupRecipients Recipients = PopupRecipients.Local;

    /// <summary>
    /// Which popup API method to use.
    /// Use PopupCoordinates in case the entity will be deleted while the popup is shown.
    /// </summary>
    [DataField]
    public PopupMethod Method = PopupMethod.PopupEntity;

    /// <summary>
    /// Size of the popup.
    /// </summary>
    [DataField]
    public PopupType VisualType = PopupType.Small;

    /// <summary>
    /// The chance of this popup actually appearing.
    /// </summary>
    [DataField]
    public float Chance = 1.0f;
}
