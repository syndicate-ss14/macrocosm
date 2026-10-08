// using Content.Shared.Localizations; MACRO
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityEffects.Effects;

/// <summary>
/// Applies the effect of an <see cref="EntityEffectPrototype"/>.
/// </summary>
public sealed partial class NestedEffect : EntityEffectBase<NestedEffect>
{
    /// <summary>
    /// The effect prototype to use.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<EntityEffectPrototype> Proto;

    // private List<string> _conditions = new(); // MACRO: commented out, not sure why this is here
    private List<string> _effects = new();

    public override string? EntityEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys)
    {
        var proto = prototype.Index(Proto);
        if (proto.GuidebookText is {} key)
            return Loc.GetString(key, ("chance", Probability));

        _effects.Clear();
        foreach (var effect in proto.Effects)
        {
            if (effect.EntityEffectGuidebookText(prototype, entSys) is not {} text)
                continue;

            // Begin MACRO: Allow hiding conditions from guidebook
            var conditions = effect.GetConditions(prototype, out int count, out var showEntry);
            if (!showEntry)
                continue;

            var desc = Loc.GetString("guidebook-nested-effect-description",
                ("effect", text),
                ("chance", effect.Probability),
                ("conditionCount", count),
                ("conditions", conditions));
            // End MACRO
            _effects.Add(desc);
        }

        return _effects.Count == 0 ? null : string.Join("\n", _effects);
    }
}

/// <summary>
/// Handles <see cref="NestedEffect"/>.
/// </summary>
public sealed partial class NestedEffectSystem : EntityEffectSystem<TransformComponent, NestedEffect>
{
    [Dependency] private SharedEntityEffectsSystem _effects = default!;

    protected override void Effect(Entity<TransformComponent> ent, ref EntityEffectEvent<NestedEffect> args)
    {
        _effects.TryApplyEffect(ent, args.Effect.Proto, args.Scale, args.User);
    }
}
