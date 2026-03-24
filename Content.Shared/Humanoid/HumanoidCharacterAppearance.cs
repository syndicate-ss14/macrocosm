using System.Linq;
using System.Numerics;
using Content.Shared.Body;
using Content.Shared.Humanoid.Markings;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Humanoid;

[DataDefinition]
[Serializable, NetSerializable]
public sealed partial class HumanoidCharacterAppearance : IEquatable<HumanoidCharacterAppearance>
{
    [DataField]
    public Color EyeColor { get; set; } = Color.Black;

    [DataField]
    public Color SkinColor { get; set; } = Color.FromHsv(new Vector4(0.07f, 0.2f, 1f, 1f));

    [DataField]
    public Dictionary<ProtoId<OrganCategoryPrototype>, Dictionary<HumanoidVisualLayers, List<Marking>>> Markings { get; set; } = new();

    public HumanoidCharacterAppearance(
        Color eyeColor,
        Color skinColor,
        Dictionary<ProtoId<OrganCategoryPrototype>, Dictionary<HumanoidVisualLayers, List<Marking>>> markings)
    {
        EyeColor = ClampColor(eyeColor);
        SkinColor = ClampColor(skinColor);
        Markings = markings;
    }

    public HumanoidCharacterAppearance(HumanoidCharacterAppearance other) :
        this(other.EyeColor, other.SkinColor, new(other.Markings))
    {

    }

    public HumanoidCharacterAppearance WithEyeColor(Color newColor)
    {
        return new(newColor, SkinColor, Markings);
    }

    public HumanoidCharacterAppearance WithSkinColor(Color newColor)
    {
        return new(EyeColor, newColor, Markings);
    }

    public HumanoidCharacterAppearance WithMarkings(Dictionary<ProtoId<OrganCategoryPrototype>, Dictionary<HumanoidVisualLayers, List<Marking>>> newMarkings)
    {
        return new(EyeColor, SkinColor, newMarkings);
    }

    public static HumanoidCharacterAppearance DefaultWithSpecies(ProtoId<SpeciesPrototype> species, Sex sex)
    {
        var protoMan = IoCManager.Resolve<IPrototypeManager>();
        var speciesPrototype = protoMan.Index<SpeciesPrototype>(species);
        var skinColoration = protoMan.Index(speciesPrototype.SkinColoration).Strategy;
        var skinColor = skinColoration.InputType switch
        {
            SkinColorationStrategyInput.Unary => skinColoration.FromUnary(speciesPrototype.DefaultHumanSkinTone),
            SkinColorationStrategyInput.Color => skinColoration.ClosestSkinColor(speciesPrototype.DefaultSkinTone),
            _ => skinColoration.ClosestSkinColor(speciesPrototype.DefaultSkinTone),
        };

        var appearance = new HumanoidCharacterAppearance(
            Color.Black,
            skinColor,
            new()
        );
        return EnsureValid(appearance, species, sex);
    }

    private static IReadOnlyList<Color> _realisticEyeColors = new List<Color>
    {
        Color.Brown,
        Color.Gray,
        Color.Azure,
        Color.SteelBlue,
        Color.Black
    };

    public static HumanoidCharacterAppearance Random(string species, Sex sex)
    {
        var random = IoCManager.Resolve<IRobustRandom>();
        var markingManager = IoCManager.Resolve<MarkingManager>();

        // TODO: Add random markings

        // MACRO START: random markings :)
        // Note that a lot of this code is modified from upstream, so some code is shared. Anything commented out is from upstream.

        List<Marking> newMarkings = [];

        // grab a completely random color.
        var baseColor = new Color(random.NextFloat(1), random.NextFloat(1), random.NextFloat(1), 1);

        // create a new color palette based on BaseColor. roll to determine what type of palette it is.
        // personally I think this should be weighted, but I can't be bothered to implement that.
        List<Color> colorPalette = [];
        switch (random.Next(3))
        {
            case 0:
                colorPalette = GetSplitComplementaries(baseColor);
                break;
            case 1:
                colorPalette = GetTriadicComplementaries(baseColor);
                break;
            case 2:
                colorPalette = GetOneComplementary(baseColor);
                break;
        }

        // var newEyeColor = random.Pick(_realisticEyeColors);

        // grab the skin type, and clamp it to our colour strategy.
        var protoMan = IoCManager.Resolve<IPrototypeManager>();
        var skinType = protoMan.Index<SpeciesPrototype>(species).SkinColoration;
        var strategy = protoMan.Index(skinType).Strategy;

        var newSkinColor = strategy.ClosestSkinColor(colorPalette[0]);

        // var newSkinColor = strategy.InputType switch
        // {
        //     SkinColorationStrategyInput.Unary => strategy.FromUnary(random.NextFloat(0f, 100f)),
        //     SkinColorationStrategyInput.Color => strategy.ClosestSkinColor(new Color(random.NextFloat(1), random.NextFloat(1), random.NextFloat(1), 1)),
        //     _ => strategy.ClosestSkinColor(new Color(random.NextFloat(1), random.NextFloat(1), random.NextFloat(1), 1)),
        // };

        // declare some defaults. ensures that the hair and eyes on hues-colored species don't match the skin or one another.
        var newHairColor = colorPalette[1];
        var newEyeColor = colorPalette[2];

        // now we do some color logic.
        if (protoMan.Index(skinType).RealisticColors)
        {
            // pick a random realistic hair color from the list and randomize it juuuuust a little bit.
            newHairColor = random.Pick(HairStyles.RealisticHairColors);
            newHairColor = newHairColor
                .WithRed(RandomizeColor(newHairColor.R))
                .WithGreen(RandomizeColor(newHairColor.G))
                .WithBlue(RandomizeColor(newHairColor.B));

            // and pick a random realistic eye color from the list.
            newEyeColor = random.Pick(_realisticEyeColors);

            // we're also going to crush the other colors down to the skin's luminosity so markings don't appear too bright on darker skin.
            // (mq comment- is this needed? could look cool without. maybe a bool?)
            colorPalette[1] = SquashToSkinLuminosity(newSkinColor, colorPalette[1]);
            colorPalette[2] = SquashToSkinLuminosity(newSkinColor, colorPalette[2]);
        }

        if (protoMan.Index(skinType).SquashAllColors)
        {
            // crush the other colors down to valid skin colors.
            colorPalette[1] = strategy.ClosestSkinColor(colorPalette[1]);
            colorPalette[2] = strategy.ClosestSkinColor(colorPalette[2]);
        }

        // declare our default hair.
        var newHairStyle = HairStyles.DefaultFacialHairStyle.Id;
        var newFacialHairStyle = HairStyles.DefaultFacialHairStyle.Id;

        // we're also grabbing a new dictionary to store our marking data on a per-organ basis.
        var markingSet = markingManager.GetMarkingData(species);

        // nubody made this logic suck so bad im so sorry.
        // now we need to somehow get from our species to a single visual layer and marking group. the only way we can do this? organs.
        // GOD I NEED A FUCKING API
        foreach (var category in protoMan.EnumeratePrototypes<OrganCategoryPrototype>())
        {
            // identify what marking group we're using for this layer,
            if (!markingSet.TryGetValue(category, out var markingData))
            {
                break;
            }

            // grab a dictionary of markings in that layer for that marking group,
            var markings = markingManager.MarkingsByLayerAndGroupAndSex(category, markingData.Group, sex);

            // and make a new dictionary that stores the string of the marking and the corresponding random weight.
            var markingWeights = new Dictionary<string, float>();
            foreach (var marking in markings)
                markingWeights.Add(marking.Key, marking.Value.RandomWeight);

            // grab the markingset from our category..
            if (!markingSet.TryGetValue(category, out var categorySet))
                continue;

            // hair and facial hair are handled different to other markings, so those get their own special treatment
            // if it's hair, and there are hair styles, roll one. else bald
            else if (category == HumanoidVisualLayers.Hair)
            {
                newHairStyle = markings.Count == 0 || !random.Prob(categorySet.Weight)
                    ? HairStyles.DefaultHairStyle.Id
                    : random.Pick(markingWeights).Key;
            }

            // if it's facial hair, there are entries in the category, and the character is not female, roll & assign a random one. else bald
            else if (category == HumanoidVisualLayers.FacialHair)
            {
                newFacialHairStyle = markings.Count == 0 || sex == Sex.Female || !random.Prob(categorySet.Weight)
                    ? HairStyles.DefaultFacialHairStyle.Id
                    : random.Pick(markingWeights).Key;
            }

            // for every other category,
            else if (markings.Keys.Any())
            {
                // add random markings!
                // this will roll once for each point in the marking category.
                for (var i = 0; i < categorySet.Limit; i++)
                {
                    // just in case there are somehow more points than markings
                    if (markingWeights.Count == 0)
                        continue;

                    // category roll to see if we add anything
                    if (!random.Prob(categorySet.Weight))
                        continue;

                    // pick a random marking from the list
                    var randomMarking = random.Pick(markingWeights).Key;
                    if (!markings.TryGetValue(randomMarking, out var protoToAdd))
                        continue;
                    var markingToAdd = protoToAdd.AsMarking();
                    Color markingColor;

                    // prevent duplicates
                    markingWeights.Remove(randomMarking);

                    // set gauze to white.
                    // side note, I really hate that gauze isn't its own category. please fix that so that i can make this not suck as much.
                    // or, like, give it its own color rules. or something.
                    if (markingToAdd.MarkingId.Contains("gauze", StringComparison.OrdinalIgnoreCase))
                    {
                        markingToAdd.SetColor(Color.White);
                        newMarkings.Add(markingToAdd);
                        continue;
                    }

                    // select a random color from our two secondary colors. if our marking is a Tail, add the skin color as well, otherwise lizards always look a little odd.
                    // this will also make moths and spiders look less interesting on average, but I don't want a hardcoded exception for lizards.
                    if (category == HumanoidVisualLayers.Tail)
                        markingColor = random.Pick(colorPalette);
                    else
                        markingColor = random.Pick(colorPalette.Skip(0).ToList());

                    // set the marking to that color
                    markingToAdd.SetColor(markingColor);

                    // otherwise, add it to the final list.
                    newMarkings.Add(markingToAdd);
                }
            }
        }

        // at the end of all that, we should have new values for each of these, so we set the character appearance to these new values.
        return new HumanoidCharacterAppearance(newHairStyle, newHairColor, newFacialHairStyle, newHairColor, newEyeColor, newSkinColor, newMarkings);

        // return new HumanoidCharacterAppearance(newEyeColor, newSkinColor, new());

        // HELPER FUNCTIONS
        // These are probably better off in Robust.Shared.Maths.Color. oh well

        float RandomizeColor(float channel)
        {
            return MathHelper.Clamp01(channel + random.Next(-25, 25) / 100f);
        }

        List<Color> GetComplementaryColors(Color color, double angle)
        {
            var hsl = Color.ToHsl(color);

            // sorry about how messy these are, but to get all random values we need to reroll for positive and negative HSL.
            var hVal = hsl.X + angle;
            hVal = hVal >= 0.360 ? hVal - 0.360 : hVal;
            var positiveHSL = new Vector4((float)hVal, MathHelper.Clamp01(hsl.Y + random.Next(-20, 0) / 100f), MathHelper.Clamp01(hsl.Z + random.Next(-15, 15) / 100f), hsl.W);

            var hVal1 = hsl.X - angle;
            hVal1 = hVal1 <= 0 ? hVal1 + 0.360 : hVal1;
            var negativeHSL = new Vector4((float)hVal1, MathHelper.Clamp01(hsl.Y + random.Next(-20, 0) / 100f), MathHelper.Clamp01(hsl.Z + random.Next(-15, 15) / 100f), hsl.W);

            var c0 = Color.FromHsl(positiveHSL);
            var c1 = Color.FromHsl(negativeHSL);

            var palette = new List<Color> { color, c0, c1 };
            return palette;
        }

        //return a list of triadic complementary colors
        List<Color> GetTriadicComplementaries(Color color)
        {
            return GetComplementaryColors(color, 0.120);
        }

        // return a list of split complementary colors
        List<Color> GetSplitComplementaries(Color color)
        {
            return GetComplementaryColors(color, 0.150);
        }

        // return a list containing the base color and two copies of a single complemenary color
        List<Color> GetOneComplementary(Color color)
        {
            return GetComplementaryColors(color, 0.180);
        }

        Color SquashToSkinLuminosity(Color skinColor, Color toSquash)
        {
            var skinColorHSL = Color.ToHsl(skinColor);
            var toSquashHSL = Color.ToHsl(toSquash);

            // check if the skin color is as dark as or darker than the marking color:
            if (toSquashHSL.Z <= skinColorHSL.Z)
            {
                // if it is, don't fuck with it
                return toSquash;
            }

            // otherwise, create a new color with the H, S, and A of toSquash, but the L of skinColor
            var newColor = new Vector4(toSquashHSL.X, toSquashHSL.Y, skinColorHSL.Z, toSquashHSL.W);
            return Color.FromHsl(newColor);
        }
        // MACRO END (whew!)
    }

    public static Color ClampColor(Color color)
    {
        return new(color.RByte, color.GByte, color.BByte);
    }

    public static HumanoidCharacterAppearance EnsureValid(HumanoidCharacterAppearance appearance, ProtoId<SpeciesPrototype> species, Sex sex)
    {
        var eyeColor = ClampColor(appearance.EyeColor);

        var proto = IoCManager.Resolve<IPrototypeManager>();
        var markingManager = IoCManager.Resolve<MarkingManager>();

        var skinColor = appearance.SkinColor;
        var validatedMarkings = appearance.Markings.ShallowClone();

        if (proto.TryIndex(species, out var speciesProto))
        {
            var strategy = proto.Index(speciesProto.SkinColoration).Strategy;
            var organs = markingManager.GetOrgans(species);
            skinColor = strategy.EnsureVerified(skinColor);

            foreach (var (organ, markings) in appearance.Markings)
            {
                if (!organs.ContainsKey(organ))
                    validatedMarkings.Remove(organ);
            }

            foreach (var (organ, organProtoID) in organs)
            {
                if (!markingManager.TryGetMarkingData(organProtoID, out var organData))
                {
                    validatedMarkings.Remove(organ);
                    continue;
                }

                var actualMarkings = appearance.Markings.GetValueOrDefault(organ)?.ShallowClone() ?? [];

                markingManager.EnsureValidColors(actualMarkings);
                markingManager.EnsureValidGroupAndSex(actualMarkings, organData.Value.Group, sex);
                markingManager.EnsureValidLayers(actualMarkings, organData.Value.Layers);
                markingManager.EnsureValidLimits(actualMarkings, organData.Value.Group, organData.Value.Layers, skinColor, eyeColor);

                validatedMarkings[organ] = actualMarkings;
            }
        }

        return new HumanoidCharacterAppearance(
            eyeColor,
            skinColor,
            validatedMarkings);
    }

    public bool Equals(HumanoidCharacterAppearance? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return EyeColor.Equals(other.EyeColor) &&
               SkinColor.Equals(other.SkinColor) &&
               MarkingManager.MarkingsAreEqual(Markings, other.Markings);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is HumanoidCharacterAppearance other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(EyeColor, SkinColor, Markings);
    }

    public HumanoidCharacterAppearance Clone()
    {
        return new(this);
    }
}
