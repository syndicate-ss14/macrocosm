using Content.Shared.Chat.TypingIndicator;
using Robust.Shared.Prototypes;

namespace Content.Shared._MACRO.Traits.Assorted;

[RegisterComponent]
public sealed partial class ReplaceTypingIndicatorComponent : Component
{
    /// <summary>
    ///     Typing indicator prototype to replace the existing one with
    /// </summary>
    [DataField("proto", required: true)]
    public ProtoId<TypingIndicatorPrototype> TypingIndicatorPrototype;
}
