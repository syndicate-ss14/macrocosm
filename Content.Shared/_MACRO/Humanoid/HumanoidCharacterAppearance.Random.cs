using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using Content.Shared.Humanoid.Markings;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared.Humanoid;

public sealed partial class HumanoidCharacterAppearance : IEquatable<HumanoidCharacterAppearance>
{
    /// <summary>
    ///     Creates a new color palette from BaseColor.
    ///     Uses integer provided to choose what kind of palette is generated.
    /// </summary>
    /// <param name="baseColor">The base color to generate a palette from.</param>
    /// <param name="strategy">0 for split complimentary, 1 for triadic complimentary, any other value for a single compliment.</param>
    /// <returns>A list of colours in the chosen palette.</returns>
    /// <remarks>
    ///     Personally I think this should be weighted, but I can't
    ///     be bothered to implement that. -widgetbeck (and mq)
    /// </remarks>
    private static List<Color> GetPaletteFromBase(Color baseColor, int strategy)
    {
        return strategy switch
        {
            0 => GetSplitComplementaries(baseColor),
            1 => GetTriadicComplementaries(baseColor),
            _ => GetOneComplementary(baseColor),
        };
    }

    /// <summary>
    ///     Clamps a 3-toned color palette (skin, hair, eyes) to the desired ISkinColorationStrategy.
    /// </summary>
    /// <returns>
    ///     A 3-toned color palette where:
    ///     0 = Skin colour,
    ///     1 = Hair colour,
    ///     2 = Eye colour.
    /// </returns>
    private static List<Color> ClampPaletteToStrategy(List<Color> colorPalette, SkinColorationPrototype skinType)
    {
        var random = IoCManager.Resolve<IRobustRandom>();

        // clamping from our generated colours instead of getting a random color.
        var newSkinColor = skinType.Strategy.ClosestSkinColor(colorPalette[0]);

        var newHairColor = colorPalette[1];
        var newEyeColor = colorPalette[2];

        if (skinType.RealisticColors)
        {
            // pick a random realistic hair color from the list and randomize it juuuuust a little bit.
            newHairColor = random.Pick(HairStyles.RealisticHairColors);
            newHairColor = newHairColor
                .WithRed(RandomizeColor(newHairColor.R))
                .WithGreen(RandomizeColor(newHairColor.G))
                .WithBlue(RandomizeColor(newHairColor.B));

            // and pick a random realistic eye color from the list.
            newEyeColor = random.Pick(_realisticEyeColors);
        }

        if (skinType.SquashAllColors)
        {
            // crush the other colors down to valid skin colors.
            newHairColor = skinType.Strategy.ClosestSkinColor(newHairColor);
            newEyeColor = skinType.Strategy.ClosestSkinColor(newEyeColor);
        }

        List<Color> outPalette = [newSkinColor, newHairColor, newEyeColor];
        return outPalette;
    }

    // hair and facial hair are handled different to other markings, so those get their own special treatment
    private static List<Marking> PickHairsRandomMarking(HumanoidVisualLayers layer, MarkingsLimits layerLimits, IReadOnlyDictionary<string, MarkingPrototype> allMarkings, Color color)
    {
        var random = IoCManager.Resolve<IRobustRandom>();

        if (allMarkings.Count == 0 || !random.Prob(layerLimits.Weight))
            return [];

        var hairId = PickWeightedMarkingId(allMarkings);
        if (hairId is null || !allMarkings.TryGetValue(hairId, out var hairProto))
            return [];

        if (allMarkings.TryGetValue(hairProto.ID, out var hairMarking))
            return [hairMarking.AsMarking().WithColor(color)];

        var protoMan = IoCManager.Resolve<IPrototypeManager>();
        var defaultHair = layer switch
        {
            HumanoidVisualLayers.FacialHair => HairStyles.DefaultFacialHairStyle,
            _ => HairStyles.DefaultHairStyle,
        };

        var defaultHairProto = protoMan.Index(defaultHair);
        return [new Marking(defaultHair, defaultHairProto.Sprites.Count).WithColor(color)];
    }

    private static List<Marking> PickLayerRandomMarkings(HumanoidVisualLayers layer, MarkingsLimits? layerLimits, IReadOnlyDictionary<string, MarkingPrototype> allMarkings, List<Color> palette)
    {

        if (layerLimits is null)
            return [];

        if (layer == HumanoidVisualLayers.Hair ||
            layer == HumanoidVisualLayers.FacialHair)
        {
            return PickHairsRandomMarking(layer, layerLimits, allMarkings, palette[1]);
        }

        var random = IoCManager.Resolve<IRobustRandom>();
        var layerWeight = layerLimits.Weight;
        var pool = allMarkings.ToDictionary();

        List<Marking> outMarkings = [];

        for (var i = 0; i < layerLimits.Limit; i++)
        {
            // just in case there are somehow more points than markings
            if (pool.Count == 0)
                break;

            // category roll to see if we add anything
            if (!random.Prob(layerWeight))
                continue;

            var randomMarking = PickWeightedMarkingId(pool);

            if (randomMarking is null || !pool.Remove(randomMarking, out var protoToAdd))
                continue;

            // select a random color from our two secondary colors.
            // TODO: we may need some color validation here. unsure.
            // TODO: multiple layers on a marking?
            var color = random.Pick(palette.Skip(0).ToList());

            outMarkings.Add(protoToAdd.AsMarking().WithColor(color));
        }
        return outMarkings;
    }

    private static string? PickWeightedMarkingId(IReadOnlyDictionary<string, MarkingPrototype> markings)
    {
        var random = IoCManager.Resolve<IRobustRandom>();

        if (markings.Count == 0)
            return null;

        var sum = 0f;
        foreach (var proto in markings.Values)
            sum += Math.Max(0f, proto.RandomWeight);

        if (sum <= 0f)
            return random.Pick(markings.Keys.ToList());

        var roll = random.NextFloat(sum);
        foreach (var (id, proto) in markings)
        {
            roll -= Math.Max(0f, proto.RandomWeight);
            if (roll <= 0f)
                return id;
        }

        return markings.Last().Key;
    }

    #region Color Helpers
    // These are probably better off in Robust.Shared.Maths.Color. oh well

    private static float RandomizeColor(float channel)
    {
        var random = IoCManager.Resolve<IRobustRandom>();
        return MathHelper.Clamp01(channel + random.Next(-25, 25) / 100f);
    }

    /// <summary>
    ///    Generates a complimentary colour palette for a provided
    ///    colour by rotating a set amount of degrees around the
    ///    colour wheel, and then varying the value and saturation
    ///    slightly.
    /// </summary>
    /// <returns>
    ///     A list of 3 colors.
    /// </returns>
    private static List<Color> GetComplementaryColors(Color color, double angle)
    {
        var random = IoCManager.Resolve<IRobustRandom>();
        var hsl = Color.ToHsl(color);

        // sorry about how messy these are, but to get all random values we need to reroll for positive and negative HSL.
        // since we want to rotate x degrees around the colour wheel, we need to do so in both directions- doing x + x degrees will give us the wrong hue!

        var hVal = hsl.X + angle;
        hVal = hVal >= 0.360 ? hVal - 0.360 : hVal;
        var positiveHSL = new Vector4(
            (float)hVal,
            MathHelper.Clamp01(hsl.Y + random.Next(-20, 0) / 100f),
            MathHelper.Clamp01(hsl.Z + random.Next(-15, 15) / 100f),
            hsl.W);

        var hVal1 = hsl.X - angle;
        hVal1 = hVal1 <= 0 ? hVal1 + 0.360 : hVal1;
        var negativeHSL = new Vector4(
            (float)hVal1,
            MathHelper.Clamp01(hsl.Y + random.Next(-20, 0) / 100f),
            MathHelper.Clamp01(hsl.Z + random.Next(-15, 15) / 100f),
            hsl.W);

        var c0 = Color.FromHsl(positiveHSL);
        var c1 = Color.FromHsl(negativeHSL);

        var palette = new List<Color> { color, c0, c1 };
        return palette;
    }

    /// <summary>
    ///     Generates a list of triadic complementary colors
    /// </summary>
    private static List<Color> GetTriadicComplementaries(Color color)
    {
        return GetComplementaryColors(color, 0.120);
    }

    /// <summary>
    ///     Generates a list of split complementary colors
    /// </summary>
    private static List<Color> GetSplitComplementaries(Color color)
    {
        return GetComplementaryColors(color, 0.150);
    }

    /// <summary>
    ///     Generates a list containing the base color and two copies of a single complementary color
    /// </summary>
    private static List<Color> GetOneComplementary(Color color)
    {
        return GetComplementaryColors(color, 0.180);
    }

    /// <summary>
    ///     Ensures that the provided colour is no lighter than the character's skin tone.
    ///     Good for tattoos and similar markings.
    /// </summary>
    /// <param name="skinColor">Reference color</param>
    /// <param name="toSquash">Colour that is being squashed</param>
    /// <returns>
    ///     A colour that is no lighter than the provided skin tone
    /// </returns>
    private static Color SquashToSkinLuminosity(Color skinColor, Color toSquash)
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
    #endregion
}
