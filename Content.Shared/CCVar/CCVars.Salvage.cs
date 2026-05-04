using Robust.Shared.Configuration;

namespace Content.Shared.CCVar;

public sealed partial class CCVars
{
    /// <summary>
    ///     Duration for missions
    /// </summary>
    public static readonly CVarDef<float>
        SalvageExpeditionDuration = CVarDef.Create("salvage.expedition_duration", 660f, CVar.REPLICATED);

    /// <summary>
    ///     Cooldown for missions.
    /// </summary>
    public static readonly CVarDef<float>
        SalvageExpeditionCooldown = CVarDef.Create("salvage.expedition_cooldown", 780f, CVar.REPLICATED);

    /// MACRCO CHANGE START///
    /// <summary>
    ///     When false, salvage reward tickets are not spawned from reclaimers, lathes, or similar ticket printers.
    /// </summary>
    public static readonly CVarDef<bool>
        SalvageTicketsEnabled = CVarDef.Create("salvage.tickets_enabled", false, CVar.SERVERONLY);        
    /// MACRCO CHANGE END///    
}

