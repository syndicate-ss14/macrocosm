// MACRO - File originally from https://github.com/cosmatic-drift-14/cosmatic-drift/pull/149, heavily modified by ZigTag for various forks, and consolidated here.
// Permalink: https://github.com/cosmatic-drift-14/cosmatic-drift/blob/4959877f52a4b0e3620a813c4dd5e26a273aff88/Content.Server/_CD/Traits/SynthSystem.cs

using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Chat.TypingIndicator;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Forensics;
using Content.Shared.Forensics.Components;
using Content.Shared.Forensics.Systems;
using Robust.Shared.Prototypes;

namespace Content.Shared._MACRO.Traits;

public sealed partial class SynthSystem : EntitySystem
{
    private static readonly ProtoId<TypingIndicatorPrototype> RobotTypingIndicator = "robot";
    private static readonly ProtoId<ReagentPrototype> SynthBlood = "SynthBlood";

    [Dependency] private BloodstreamSystem _bloodstream = default!;
    [Dependency] private SharedTypingIndicatorSystem _typingIndicator = default!;

    [SubscribeLocalEvent(after: [typeof(ForensicsSystem)])]
    private void OnStartup(EntityUid uid, SynthComponent component, MapInitEvent args)
    {
        _typingIndicator.SetIndicatorPrototype(uid, RobotTypingIndicator);

        if (!TryComp<BloodstreamComponent>(uid, out var bloodstreamComponent))
            return;

        var bloodstreamVolume = bloodstreamComponent.BloodReferenceSolution.Volume;

        var synthBloodSolution = new Solution(SynthBlood, bloodstreamVolume);

        // Give them synth blood. Ion storm notif is handled in that system
        _bloodstream.ChangeBloodReagents(uid, synthBloodSolution);

        if (!TryComp<DnaComponent>(uid, out var dnaComp) || dnaComp.DNA == null)
            return;

        var ev = new GenerateDnaEvent { Owner = uid, DNA = dnaComp.DNA };
        RaiseLocalEvent(uid, ref ev);
    }
}
