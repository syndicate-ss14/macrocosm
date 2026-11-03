using Robust.Shared.Configuration;

namespace Content.Shared._MACRO.CCVar;

// Used for variables that affect characters primarily or are used to enforce roleplay standards.

public sealed partial class MacroCCVars
{
    /// <summary>
    ///     The default species weights used for randomly-generated humanoids (such as ghost-role visitors).
    ///     Set to empty or an invalid prototype ID to select from roundstart species (unweighted) instead.
    /// </summary>
    public static readonly CVarDef<string> VisitorSpeciesWeights =
        CVarDef.Create("macrocosm.ic.visitor_species_weights", "VisitorSpeciesWeights", CVar.SERVERONLY);
}
