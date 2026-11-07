using Content.Shared.Chemistry;
using Content.Shared.Metabolism;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.EntitySystems;

public sealed partial class ChemistryGuideDataSystem
{
    /// <summary>
    ///     Attempt to reload all reagent prototypes conditionally.
    /// </summary>
    /// <returns>Whether or not all prototypes had to be reloaded (and thus this should exit early.)</returns>
    private bool TryReloadAllTypes(PrototypesReloadedEventArgs args)
    {
        if (!args.WasModified<MetabolizerTypePrototype>())
            return false;

        ReloadAllReagentPrototypes();
        return true;
    }
}
