using System.Linq;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.EntityEffects;
using Content.Shared.Localizations;
using Content.Shared.Metabolism;
using Robust.Shared.Prototypes;

namespace Content.Shared.EntityConditions.Conditions.Body;

/// <inheritdoc cref="EntityCondition"/>
public sealed partial class MetabolizerTypeCondition : EntityConditionBase<MetabolizerTypeCondition>
{
    /// <summary>
    /// Which metabolizer types would fulfill this condition. Need only one match.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<MetabolizerTypePrototype>[] Type = default!;

    public override string EntityConditionGuidebookText(IPrototypeManager prototype)
    {
        var typeList = new List<string>();
        var allHidden = Type.Length > 0; // MACRO: Keep track if all metabolizers are hidden from the guidebook

        foreach (var type in Type)
        {
            if (!prototype.Resolve(type, out var proto))
                continue;

            // MACRO: Do not show this metabolizer type if it's hidden from the guidebook.
            allHidden = allHidden && !proto.ShowInGuidebook;
            if (!proto.ShowInGuidebook)
                continue;

            typeList.Add(proto.LocalizedName);
        }

        // Begin MACRO: This requirement gets hidden entirely if all metabolizers are hidden.
        if (allHidden)
        {
            // Pretend there's no conditions if this is inverted, because it hypothetically should never be "failed".
            // "Adds 5 seconds of status effect [if the metabolizing organ is not an Allulalo organ.]"
            if (Inverted)
                return string.Empty;
            // The whole effect should be hidden if it's not inverted, because it hypothetically should never pass.
            // "Adds 5 seconds of status effect [if the metabolizing organ is an Allulalo organ.]"
            else
                return EntityEffect.HideEffectTag;
        }
        // End MACRO

        var names = ContentLocalizationManager.FormatListToOr(typeList);

        return Loc.GetString("entity-condition-guidebook-organ-type",
            ("name", names),
            ("shouldhave", !Inverted));
    }
}

/// <summary>
/// Returns true if this entity has any of the listed metabolizer types.
/// </summary>
/// <inheritdoc cref="EntityConditionSystem{T, TCondition}"/>
public sealed partial class MetabolizerTypeEntityConditionSystem : EntityConditionSystem<MetabolizerComponent, MetabolizerTypeCondition>
{
    protected override void Condition(Entity<MetabolizerComponent> entity, ref EntityConditionEvent<MetabolizerTypeCondition> args)
    {
        if (entity.Comp.MetabolizerTypes == null)
            return;

        args.Result = entity.Comp.MetabolizerTypes.Overlaps(args.Condition.Type);
    }
}
