using Robust.Shared.Configuration;

namespace Content.Shared._MACRO.MCCVar;

// This is the first file made, so I'll put it here.
// Why MCCVars?
// M stands for Macro, so imports are a lot nicer.
[CVarDefs]
public sealed partial class MCCVars
{
    /// <summary>
    /// This is what currently adminned players hear when they get an ahelp.
    /// </summary>
    public static readonly CVarDef<string> AHelpAdminSound =
        CVarDef.Create("audio.ahelp_admin_sound", "/Audio/Effects/adminhelp.ogg", CVar.ARCHIVE | CVar.CLIENTONLY);
}
