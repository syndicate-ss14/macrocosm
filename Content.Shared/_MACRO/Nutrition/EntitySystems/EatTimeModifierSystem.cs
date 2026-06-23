using Content.Shared._MACRO.Nutrition.Components;
using Content.Shared.Item;
using Content.Shared.Nutrition;
using Content.Shared.Nutrition.EntitySystems;

namespace Content.Shared._MACRO.Nutrition.EntitySystems;

/// <summary>
/// Handles the time modification of the eaten food
/// </summary>
public sealed class EatTimeModifierSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<EdibleEvent>(OnEdible, after: [typeof(IngestionSystem)]);
    }

    private void OnEdible(ref EdibleEvent args)
    {
        if (args.Cancelled)
            return;

        if (!TryComp<EatTimeModifierComponent>(args.User, out var comp))
            return;

        args.Time *= comp.Multiplier;
    }
}
