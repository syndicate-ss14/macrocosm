using Robust.Shared.Configuration;

namespace Content.Shared._MACRO.MCCVars;

[CVarDefs]
public sealed partial class MCCVars
{
    /// <summary>
    ///     The prototype to use for announcer weights.
    /// </summary>
    public static readonly CVarDef<string> AnnouncerWeightPrototype =
        CVarDef.Create("macro.announcer_weight_prototype", "Announcers", CVar.SERVERONLY);
}
