using Content.Shared.EntityConditions;
using Content.Shared.StatusEffectNew;
using Content.Shared.StatusEffectNew.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared._MACRO.EntityCondiitons.Conditions.StatusEffects;

/// <summary>
/// Returns true if this entity has a status effect within a certain range of durations.
/// </summary>
/// <inheritdoc cref="EntityConditionSystem{T, TCondition}"/>
public sealed partial class StatusEffectDurationEntityConditionSystem
    : EntityConditionSystem<MetaDataComponent, StatusEffectDurationCondition>
{
    [Dependency] private StatusEffectsSystem _statusEffects = default!;

    protected override void Condition(Entity<MetaDataComponent> entity,
        ref EntityConditionEvent<StatusEffectDurationCondition> args)
    {
        if (!_statusEffects.TryGetStatusEffect(entity, args.Condition.EffectProto, out var effect)
            || !TryComp<StatusEffectComponent>(effect, out var statusEffect))
        {
            args.Result = args.Condition.Min <= TimeSpan.Zero;
            return;
        }

        args.Result = statusEffect.Duration >= args.Condition.Min
            && statusEffect.Duration <= args.Condition.Max;
    }
}

/// <summary>
///     Represents an entity having a status effect within a certain range of durations.
/// </summary>
public sealed partial class StatusEffectDurationCondition : EntityConditionBase<StatusEffectDurationCondition>
{
    [DataField]
    public TimeSpan Min = TimeSpan.Zero;

    [DataField]
    public TimeSpan Max = TimeSpan.MaxValue;

    [DataField(required: true)]
    public EntProtoId EffectProto;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype)
    {
        if (!prototype.Resolve(EffectProto, out var effectProto))
            return string.Empty;

        return Loc.GetString("entity-condition-guidebook-status-effect-duration",
            ("effect", effectProto.Name),
            ("max", Max == TimeSpan.MaxValue ? int.MaxValue : Max.TotalSeconds),
            ("min", Min.TotalSeconds));
    }
}
