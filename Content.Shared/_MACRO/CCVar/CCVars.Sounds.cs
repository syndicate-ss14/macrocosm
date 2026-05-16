using Robust.Shared.Configuration;

namespace Content.Shared._MACRO.CCVar;

[CVarDefs]
public sealed partial class CCVars
{
    /// <summary>
    /// This is what currently adminned players hear when they get an ahelp.
    /// </summary>
    public static readonly CVarDef<string> AHelpAdminSound =
        CVarDef.Create("audio.ahelp_admin_sound", "/Audio/Effects/adminhelp.ogg", CVar.ARCHIVE | CVar.CLIENTONLY);
}
