// Derived from synth trait from Cosmatic Drift from PR https://github.com/cosmatic-drift-14/cosmatic-drift/pull/149

using Content.Shared.Body.Components;
using Content.Shared.Body.Systems;
using Content.Shared.Chemistry.Components;
using Content.Shared.Forensics;
using Content.Shared.Forensics.Components;
using Content.Shared.Forensics.Systems;

namespace Content.Shared._MACRO.Traits.Assorted;

public sealed partial class ReplaceBloodstreamSystem : EntitySystem
{
    [Dependency] private BloodstreamSystem _bloodstream = default!;

    [SubscribeLocalEvent(after: [typeof(ForensicsSystem)])]
    private void OnStartup(Entity<ReplaceBloodstreamComponent> ent, ref MapInitEvent args)
    {
        if (!TryComp<BloodstreamComponent>(ent, out var bloodstreamComponent))
            return;

        var bloodstreamVolume = bloodstreamComponent.BloodReferenceSolution.Volume;

        var synthBloodSolution = new Solution(ent.Comp.ReagentPrototype, bloodstreamVolume);

        // Give them synth blood. Ion storm notif is handled in that system
        _bloodstream.ChangeBloodReagents(ent.Owner, synthBloodSolution);

        if (!TryComp<DnaComponent>(ent, out var dnaComp) || dnaComp.DNA == null)
            return;

        var ev = new GenerateDnaEvent { Owner = ent, DNA = dnaComp.DNA };
        RaiseLocalEvent(ent, ref ev);
    }
}
