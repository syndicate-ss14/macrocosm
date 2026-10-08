using Content.Shared.Item.ItemToggle.Components;
using Content.Shared.Whitelist;

namespace Content.Shared.Medical;

/// <summary>
/// This is used for defibrillators; a machine that shocks a dead
/// person back into the world of the living.
/// Uses <see cref="ItemToggleComponent"/>
/// </summary>
public sealed partial class DefibrillatorComponent : Component
{
    /// <summary>
    /// Macrocosm: If not null, defib will only work on entities which pass the whitelist check
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityWhitelist? Whitelist;

    /// <summary>
    /// Macrocosm: Should we show the popup messages when zapping?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool ShowMessages = true;

    /// <summary>
    /// Macrocosm: If the entity is healthier than its crit threshold (or has no crit threshold) should we allow it to
    /// go straight to "Alive"?
    /// </summary>
    [DataField, AutoNetworkedField]
    public bool AllowBypassCrit = false;
}

