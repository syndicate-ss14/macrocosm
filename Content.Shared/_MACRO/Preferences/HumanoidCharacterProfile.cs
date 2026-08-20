using System.Linq;
using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Random;
using Content.Shared.Random.Helpers;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Preferences;

public partial class HumanoidCharacterProfile
{
    /// <summary>
    ///     Attempts to pick a random species from a weighted species prototype.
    /// </summary>
    /// <param name="weightsId">A definition of species weights to use.</param>
    /// <param name="ignoredSpecies">Optional species to exclude from selection.</param>
    public static SpeciesPrototype? RandomSpeciesWeighted(ProtoId<WeightedRandomSpeciesPrototype> weightsId,
        HashSet<string>? ignoredSpecies = null,
        IPrototypeManager? protoMan = null)
    {
        protoMan ??= IoCManager.Resolve<IPrototypeManager>();

        if (!protoMan.TryIndex(weightsId, out var weightedSpecies))
            return null;

        return RandomSpeciesWeighted(weightedSpecies.Weights, ignoredSpecies, protoMan);
    }

    /// <summary>
    ///     Attempts to pick a random species from a dictionary mapping species IDs to floats..
    /// </summary>
    /// <param name="weights">A dictionary of species weights.</param>
    /// <param name="ignoredSpecies">Optional species to exclude from selection.</param>
    public static SpeciesPrototype? RandomSpeciesWeighted(Dictionary<string, float> weights,
        HashSet<string>? ignoredSpecies = null,
        IPrototypeManager? protoMan = null)
    {
        protoMan ??= IoCManager.Resolve<IPrototypeManager>();
        var random = IoCManager.Resolve<IRobustRandom>();

        // Make a copy to avoid editing the original prototype.
        var copiedWeights = new Dictionary<string, float>(weights);

        // Remove ignored species.
        if (ignoredSpecies != null)
        {
            foreach (var species in ignoredSpecies)
                copiedWeights.Remove(species);
        }

        // Pick a random species.
        var speciesId = random.Pick(weights);
        protoMan.TryIndex<SpeciesPrototype>(speciesId, out var pickedSpecies);

        return pickedSpecies;
    }

    /// <summary>
    ///     Picks a random species from a weighted list, or roundstart species if this is not possible.
    /// </summary>
    /// <param name="ignoredSpecies">Species to exclude from randomizer.</param>
    /// <param name="weightsId">A definition of species weights to use.</param>
    public static SpeciesPrototype RandomSpecies(ProtoId<WeightedRandomSpeciesPrototype> weightsId,
        HashSet<string>? ignoredSpecies = null)
    {
        var weighted = RandomSpeciesWeighted(weightsId, ignoredSpecies);
        if (weighted != null)
            return weighted;

        return RandomSpecies(ignoredSpecies);
    }

    /// <summary>
    ///     Generates a randomized character profile, using provided random species weights.
    /// </summary>
    /// <returns>A new character profile with values randomized.</returns>
    public static HumanoidCharacterProfile Random(ProtoId<WeightedRandomSpeciesPrototype> weightsId,
        HashSet<string>? ignoredSpecies = null)
    {
        var config = RandomizeConfigAll;
        var baseProfile = new HumanoidCharacterProfile();

        // If ignoredSpecies is empty, then it will choose a random species -
        // but if it's null, it'll just use humans instead!
        // This is to preserve upstream behavior, which does the same thing.
        if (ignoredSpecies != null)
            baseProfile.Species = RandomSpecies(weightsId, ignoredSpecies);

        var profile = Random(config, baseProfile);
        return profile;
    }
}
