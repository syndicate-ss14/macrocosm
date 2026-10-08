using Content.Shared.Random;
using Robust.Shared.Prototypes;

namespace Content.Shared.Humanoid.Prototypes;

public sealed partial class RandomHumanoidSettingsPrototype
{
    /// <summary>
    ///     An optional definition of random species weights to use.
    /// </summary>
    [DataField]
    public ProtoId<WeightedRandomSpeciesPrototype>? SpeciesWeights = null;
}
