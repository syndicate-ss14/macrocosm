using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared._Monkestation.Announcements;

/// <summary>
/// This is a prototype for announcer variants
/// </summary>
[Prototype("msAnnouncer")]
public sealed partial class MSAnnouncerPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public required Dictionary<ProtoId<MSAnnouncementSoundPrototype>, SoundSpecifier> Sounds;
}
