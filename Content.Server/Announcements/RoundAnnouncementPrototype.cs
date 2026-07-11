using Content.Shared._Monkestation.Announcements;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.Announcements;

/// <summary>
/// Used for any announcements on the start of a round.
/// </summary>
[Prototype]
public sealed partial class RoundAnnouncementPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField("sound")] public ProtoId<MSAnnouncementSoundPrototype>? Sound; // Monkestation edit announcement override

    [DataField("message")] public string? Message;
}
