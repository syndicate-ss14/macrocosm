using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Content.Shared._Monkestation.Announcements;
using Content.Shared._Monkestation.CCVars;
using Content.Shared.Random;
using Robust.Shared.Audio;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server._Monkestation.Announcements;

public sealed partial class AnnouncerManager : IPostInjectInit
{
    [Dependency] private IPrototypeManager _prototypeManager = default!;
    [Dependency] private IConfigurationManager _configurationManager = default!;
    [Dependency] private ILogManager _logManager = default!;
    [Dependency] private IRobustRandom _random = default!;

    private static readonly ProtoId<MSAnnouncerPrototype> DefaultAnnouncer = "DefaultAnnouncer";

    [ViewVariables(VVAccess.ReadWrite)]
    private ProtoId<MSAnnouncerPrototype> _announcerId = DefaultAnnouncer;

    private ISawmill _sawmill = default!;

    /// <summary>
    /// Attempts to get the sound specifier for an announcement from the current announcer
    /// </summary>
    /// <param name="announcementId">The ID of the announcement sound</param>
    /// <param name="soundSpecifier">The sound specifier to play</param>
    /// <returns>True if the sound was found</returns>
    public bool TryGetAnnouncerSound(ProtoId<MSAnnouncementSoundPrototype> announcementId,
        [NotNullWhen(true)] out SoundSpecifier? soundSpecifier)
    {
        var announcer = _prototypeManager.Index(_announcerId);
        if (announcer.Sounds.TryGetValue(announcementId, out soundSpecifier))
            return true;

        if (!_prototypeManager.TryIndex(announcementId, out var announcement))
            return false;
        soundSpecifier = announcement.DefaultSound;
        return true;
    }

    /// <summary>
    /// Randomizes the announcer for the round
    /// </summary>
    public void RandomizeAnnouncer()
    {
        ProtoId<WeightedRandomPrototype> weights = _configurationManager.GetCVar(MonkeCCVars.AnnouncerWeightPrototype);
        if (!TryPickAnnouncer(weights, out _announcerId))
            _sawmill.Error("Failed to load announcer.");
    }

    /// <summary>
    /// Attempts to pick an announcer from the random weights, defaults to DefaultAnnouncer
    /// </summary>
    /// <param name="weights">The weighted random prototype to use</param>
    /// <param name="announcerId">The selected announcer</param>
    /// <returns>True if selection succeeded</returns>
    private bool TryPickAnnouncer(ProtoId<WeightedRandomPrototype> weights,
        out ProtoId<MSAnnouncerPrototype> announcerId)
    {
        announcerId = DefaultAnnouncer;
        if (!_prototypeManager.TryIndex(weights, out var options))
        {
            _sawmill.Error($"Unknown weighted random prototype {weights}");
            return false;
        }

        var validWeights = options.Weights.Where(pair =>
        {
            if (_prototypeManager.HasIndex<MSAnnouncerPrototype>(pair.Key))
                return true;
            _sawmill.Error($"Unknown announcer prototype {pair.Key}");
            return false;

        })
            .ToDictionary();
        if (validWeights.Count == 0)
        {
            _sawmill.Error($"No announcers found in weights {weights}.");
        }

        var total = validWeights.Values.Sum();
        var accumulated = 0f;
        var rand = _random.NextFloat(total);
        foreach (var (key, weight) in validWeights)
        {
            accumulated += weight;
            if (accumulated < rand)
                continue;
            announcerId = key;
            return true;
        }
        _sawmill.Error("Weighted random selection failed");
        return false;
    }

    public void PostInject()
    {
        _sawmill = _logManager.GetSawmill("announcer_manager");
    }
}
