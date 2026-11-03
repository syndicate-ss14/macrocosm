// Derived from synth trait from Cosmatic Drift from PR https://github.com/cosmatic-drift-14/cosmatic-drift/pull/149

using Content.Shared.Chemistry.Reagent;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._MACRO.Traits.Assorted;

/// <summary>
/// Component which allows you to replace a bloodstream as a trait
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(ReplaceBloodstreamSystem))]
public sealed partial class ReplaceBloodstreamComponent : Component
{
    /// <summary>
    ///     Prototype ID of the reagent you would like as your bloodstream reference
    /// </summary>
    [DataField("proto", required: true)]
    public ProtoId<ReagentPrototype> ReagentPrototype;
}
