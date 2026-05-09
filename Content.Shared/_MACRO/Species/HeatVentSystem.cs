using Content.Shared.Actions;
using Content.Shared.Alert;
using Content.Shared.Atmos;
using Content.Shared.Atmos.EntitySystems;
using Content.Shared.Damage.Systems;
using Content.Shared.DoAfter;
using Content.Shared.Popups;
using Robust.Shared.Audio.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Timing;

namespace Content.Shared._MACRO.Species;

public abstract class HeatVentSystem : EntitySystem
{
    [Dependency] private readonly AlertsSystem _alerts = default!;
    [Dependency] private readonly DamageableSystem _damage = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly SharedAtmosphereSystem _atmos = default!;
    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HeatVentComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<HeatVentComponent, HeatVentActionEvent>(OnVentStart);
        SubscribeLocalEvent<HeatVentComponent, HeatVentDoAfterEvent>(OnVentEnd);

    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<HeatVentComponent>();
        while (query.MoveNext(out var uid, out var comp))
        {
            if (_timing.CurTime < comp.UpdateTimer)
                continue;

            comp.UpdateTimer = _timing.CurTime + comp.UpdateCooldown;

            Cycle((uid, comp));
        }
    }

    /// <summary>
    ///     Adds more heat to the entity's component if this entity is breathing.
    ///     Will also deal damage if the heat stored exceeds threshold.
    /// </summary>
    public void Cycle(Entity<HeatVentComponent> ent)
    {
        // TODO: update respiration
        // if (!TryComp<RespiratorComponent>(ent, out var respirator) || !_respirator.IsBreathing((ent.Owner, respirator)))
        //     return;

        ent.Comp.HeatStored += ent.Comp.HeatAdded;

        if (ent.Comp.HeatStored >= ent.Comp.HeatDamageThreshold)
            _damage.TryChangeDamage(ent.Owner, ent.Comp.HeatDamage, ignoreResistances: true, interruptsDoAfters: false);

        UpdateAlert(ent);
    }

    private void OnStartup(Entity<HeatVentComponent> ent, ref ComponentStartup args)
    {
        _actions.AddAction(ent, ent.Comp.VentAction);
    }

    /// <summary>
    ///     Run when the VentAction is used.
    /// </summary>
    private void OnVentStart(Entity<HeatVentComponent> ent, ref HeatVentActionEvent args)
    {
        if (args.Handled)
            return;

        var doAfter = new DoAfterArgs(EntityManager,
            ent,
            Math.Clamp(ent.Comp.HeatStored * ent.Comp.VentLengthMultiplier, ent.Comp.VentLengthMin, ent.Comp.VentLengthMax),
            new HeatVentDoAfterEvent(),
            ent)
        {
            BlockDuplicate = true,
        };

        if (_doAfter.TryStartDoAfter(doAfter))
            _popup.PopupEntity(Loc.GetString(ent.Comp.VentStartPopup, ("target", ent)), ent);

        args.Handled = true;
    }

    /// <summary>
    ///     Run when the venting doafter ends.
    ///     Releases steam into the surrounding tiles and reduces
    ///     entity's stored heat to 0.
    /// </summary>
    private void OnVentEnd(Entity<HeatVentComponent> ent, ref HeatVentDoAfterEvent args)
    {
        // need to make a new mixture instead of adjusting tile directly so we can modify heat
        var gasTemp = ent.Comp.GasTempBase + ent.Comp.GasTempHeatMultiplier * ent.Comp.HeatStored;
        GasMixture tileMix = new()
        {
            Volume = ent.Comp.HeatStored * ent.Comp.MolesPerHeatStored,
            Temperature = Math.Clamp(gasTemp, ent.Comp.GasTempMin, ent.Comp.GasTempMax)
        };

        _atmos.MergeTileMixture((ent, Transform(ent)), tileMix, excite: true);

        _audio.PlayPredicted(ent.Comp.VentSound, ent, ent);
        _popup.PopupPredicted(Loc.GetString(ent.Comp.VentDoAfterPopup, ("target", ent)), ent, ent);
        ent.Comp.HeatStored = 0;
        UpdateAlert(ent);
    }

    private void UpdateAlert(Entity<HeatVentComponent> ent)
    {
        short severity;
        switch (ent.Comp.HeatStored / ent.Comp.HeatDamageThreshold)
        {
            case >= 1f:
                severity = 5;
                break;
            case >= 0.6f:
                severity = 4;
                break;
            case >= 0.3f:
                severity = 3;
                break;
            case >= 0.15f:
                severity = 2;
                break;
            default:
                severity = 1;
                break;
        }

        if (TryComp<AlertsComponent>(ent, out var alerts))
        {
            _alerts.ShowAlert((ent, alerts), ent.Comp.Alert, severity);
        }
    }

}

/// <summary>
///     Relayed upon using heat vent action.
/// </summary>
public sealed partial class HeatVentActionEvent : InstantActionEvent;

/// <summary>
///     Is relayed after the doafter finishes.
/// </summary>
[Serializable, NetSerializable]
public sealed partial class HeatVentDoAfterEvent : SimpleDoAfterEvent;
