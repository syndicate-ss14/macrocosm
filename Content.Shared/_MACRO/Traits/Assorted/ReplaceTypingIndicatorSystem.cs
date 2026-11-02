using Content.Shared.Chat.TypingIndicator;

namespace Content.Shared._MACRO.Traits.Assorted;

public sealed partial class ReplaceTypingIndicatorSystem : EntitySystem
{
    [Dependency] private SharedTypingIndicatorSystem _typingIndicator = default!;

    [SubscribeLocalEvent]
    private void OnMapInit(Entity<ReplaceTypingIndicatorComponent> ent, ref MapInitEvent args)
    {
        _typingIndicator.SetIndicatorPrototype(ent, ent.Comp.TypingIndicatorPrototype);
    }
}
