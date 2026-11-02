using Content.Shared.Body.Components; // MACRO
using Content.Shared.Body.Systems; // MACRO - Move synthetic trait to shared
using Content.Shared.Chat.TypingIndicator;
using Content.Shared.Chemistry.Components; // MACRO
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Forensics; // MACRO
using Content.Shared.Forensics.Components; // MACRO
using Robust.Shared.Prototypes;

namespace Content.Shared._CD.Traits; // MACRO - Move synthetic trait to shared

public sealed partial class SynthSystem : EntitySystem
{
    private static readonly ProtoId<TypingIndicatorPrototype> RobotTypingIndicator = "robot"; // MACRO - Type safety
    private static readonly ProtoId<ReagentPrototype> SynthBlood = "SynthBlood"; // MACRO - Type safety

    [Dependency] private BloodstreamSystem _bloodstream = default!;
    [Dependency] private SharedTypingIndicatorSystem _typingIndicator = default!; // MACRO - Partial typing indicator change

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SynthComponent, ComponentStartup>(OnStartup);
    }

    private void OnStartup(EntityUid uid, SynthComponent component, ComponentStartup args)
    {
        _typingIndicator.SetIndicatorPrototype(uid, RobotTypingIndicator); // MACRO - Type safety and partial typing indicator change

        // MACRO Start - Update for new bloodstream system
        if (!TryComp<BloodstreamComponent>(uid, out var bloodstreamComponent))
            return;

        var bloodstreamVolume = bloodstreamComponent.BloodReferenceSolution.Volume;

        var synthBloodSolution = new Solution(SynthBlood, bloodstreamVolume);
        // MACRO End

        // Give them synth blood. Ion storm notif is handled in that system
        _bloodstream.ChangeBloodReagents(uid, synthBloodSolution); // MACRO - Update to ChangeBloodReagents

        // MACRO Start - Regenerate DNA
        if (!TryComp<DnaComponent>(uid, out var dnaComp) || dnaComp.DNA == null)
            return;

        var ev = new GenerateDnaEvent { Owner = uid, DNA = dnaComp.DNA };
        RaiseLocalEvent(uid, ref ev);
        // MACRO End
    }
}
