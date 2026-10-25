using Content.Server.Atmos.Piping.Unary.EntitySystems;
using Content.Shared.Chemistry.Components;

namespace Content.Server.Atmos.Piping.Unary.Components;

/// <summary>
/// Used for an entity that converts moles of gas into units of reagent.
/// </summary>
[RegisterComponent]
[Access(typeof(GasCondenserSystem))]
public sealed partial class GasCondenserComponent : Component
{
    /// <summary>
    /// The ID for the pipe node.
    /// </summary>
    [DataField]
    public string Inlet = "pipe";

    /// <summary>
    /// The ID for the solution.
    /// </summary>
    [DataField]
    public string SolutionId = "tank";

    /// <summary>
    /// The solution that gases are condensed into.
    /// </summary>
    [ViewVariables]
    public Entity<SolutionComponent>? Solution = null;

    /// <summary>
    /// For a condenser, how many U of reagents are given per each mole of gas.
    /// </summary>
    /// <remarks>
    /// Derived from a standard of 500u per canister:
    /// 400u / 1871.71051 moles per canister
    /// </remarks>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float MolesToReagentMultiplier = 0.2137f;

    // MACRO ADD START
    /// <summary>
    /// How slowly the condenser operates. It is balanced such that the condenser will produce about 1u per second.
    /// </summary>
    /// <remarks>
    /// Balanced to produce 1u/s around the median gas specific heat, which happens to be nitrogen.
    /// Note that different gasses have different specific heats, so some will be processed faster or slower.
    /// </remarks>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public float Apathy = 285f;
    // MACRO ADD END
}
