using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Shared._Monkestation.Announcements;

/// <summary>
/// This is a prototype for an announcement sound
/// </summary>
[Prototype("msAnnouncementSound")]
public sealed partial class MSAnnouncementSoundPrototype : IPrototype
{
    /// <inheritdoc/>
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField(required: true)]
    public required SoundSpecifier DefaultSound;
}
