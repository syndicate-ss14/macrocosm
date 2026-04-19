using System.Diagnostics;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Atmos.Piping.Components;
using Content.Server.Atmos.Piping.Unary.Components;
using Content.Server.NodeContainer;
using Content.Server.NodeContainer.EntitySystems;
using Content.Server.NodeContainer.Nodes;
using Content.Server.Power.Components;
using Content.Server.Power.EntitySystems;
using Content.Shared.Atmos;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.FixedPoint;
using JetBrains.Annotations;

namespace Content.Server.Atmos.Piping.Unary.EntitySystems;

[UsedImplicitly]
public sealed class GasCondenserSystem : EntitySystem
{
    [Dependency] private readonly AtmosphereSystem _atmosphereSystem = default!;
    [Dependency] private readonly PowerReceiverSystem _power = default!;
    [Dependency] private readonly NodeContainerSystem _nodeContainer = default!;
    [Dependency] private readonly SharedSolutionContainerSystem _solution = default!;

    // MACRO EDIT START


    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GasCondenserComponent, AtmosDeviceUpdateEvent>(OnCondenserUpdated);
    }

    private void OnCondenserUpdated(Entity<GasCondenserComponent> entity, ref AtmosDeviceUpdateEvent args)
    {
        if (!(TryComp<ApcPowerReceiverComponent>(entity, out var receiver) && _power.IsPowered(entity, receiver))
            || !_nodeContainer.TryGetNode(entity.Owner, entity.Comp.Inlet, out PipeNode? inlet)
            || !_solution.ResolveSolution(entity.Owner, entity.Comp.SolutionId, ref entity.Comp.Solution, out var solution))
        {
            return;
        }

        if (solution.AvailableVolume == 0 || inlet.Air.TotalMoles == 0)
            return;

        var molesToConvert = NumberOfMolesToConvert(receiver, inlet.Air, args.dt, entity); // MACRO: pass in entity
        var removed = inlet.Air.Remove(molesToConvert);
        for (var i = 0; i < Atmospherics.TotalNumberOfGases; i++)
        {
            var moles = removed[i];
            if (moles <= 0)
                continue;

            if (_atmosphereSystem.GetGas(i).Reagent is not { } gasReagent)
                continue;

            var moleToReagentMultiplier = entity.Comp.MolesToReagentMultiplier;
            var amount = FixedPoint2.Min(FixedPoint2.New(moles * moleToReagentMultiplier), solution.AvailableVolume);
            if (amount <= 0)
                continue;

            solution.AddReagent(gasReagent, amount);

            // if we have leftover reagent, then convert it back to moles and put it back in the mixture.
            inlet.Air.AdjustMoles(i, moles - (amount.Float() / moleToReagentMultiplier));
        }

        _solution.UpdateChemicals(entity.Comp.Solution.Value);
    }

    // BEGIN MACRO ADD
    public float NumberOfMolesToConvert(ApcPowerReceiverComponent comp, GasMixture mix, float dt, Entity<GasCondenserComponent> entity)
    {
        //Rate of condensation is based on the gas mixture's specific heat (not heat capacity!).
        var specificHeat = _atmosphereSystem.GetSpecificHeat(mix);

        //Power usage of the condenser since the last update.
        //Generally going to be a constant 6000.
        var energy = comp.Load * dt;
        Log.Info($"energy {energy}");

        //Apathy should not be 0
        var apathy = entity.Comp.Apathy == 0f ? 0.0001f : entity.Comp.Apathy;

        return energy / (apathy * specificHeat);
    }
    // END MACRO ADD

    /* MACRO EDIT: UPSTREAM IMPLEMENTATION
    public float NumberOfMolesToConvert(ApcPowerReceiverComponent comp, GasMixture mix, float dt)
    {
        var hc = _atmosphereSystem.GetHeatCapacity(mix, true);
        var alpha = 0.8f; // tuned to give us 1-ish u/second of reagent conversion
        // ignores the energy needed to cool down the solution to the condensation point, but that probably adds too much difficulty and so let's not simulate that
        var energy = comp.Load * dt;
        return energy / (alpha * hc);
    }
    */
}
