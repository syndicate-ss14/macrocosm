using Content.Server._MACRO.Announcements;
using Content.Server.Chat.Managers;
using Content.Shared.Chat;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.StationEvents.Events;

/// <summary>
///     This is a derivation of the ion storm notifier from Cosmatic Drift PR https://github.com/cosmatic-drift-14/cosmatic-drift/pull/149
/// </summary>
public sealed partial class IonStormRule
{
    [Dependency] private IChatManager _chatManager = default!;
    [Dependency] private IRobustRandom _random = default!;

    private void NotifyAllIonStorm()
    {
        var notifierQuery = EntityQueryEnumerator<IonStormNotifierComponent>();
        while (notifierQuery.MoveNext(out var ent, out var notifierComponent))
        {
            NotifyIonStorm(ent, notifierComponent.AlertChance, notifierComponent.Loc);
        }
    }

    private void NotifyIonStorm(EntityUid ent, float alertChance, string loc)
    {
        if (!_random.Prob(alertChance))
            return;

        if (!TryComp<ActorComponent>(ent, out var actor))
            return;

        var msg = Loc.GetString(loc);
        var wrappedMessage = Loc.GetString("chat-manager-server-wrap-message", ("message", msg));
        _chatManager.ChatMessageToOne(ChatChannel.Server, msg, wrappedMessage, default, false, actor.PlayerSession.Channel, colorOverride: Color.Yellow);
    }
}
