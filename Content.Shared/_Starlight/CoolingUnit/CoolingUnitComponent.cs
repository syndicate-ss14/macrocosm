using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.CoolingUnit;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class CoolingUnitComponent : Component
{
    [DataField]
    public EntProtoId ToggleAction = "ActionToggleCoolingUnit";

    [DataField, AutoNetworkedField]
    public EntityUid? ToggleActionEntity;

    /// <summary>
    /// Max Cooling per second, in degrees Kelvin.
    /// </summary>
    [DataField]
    public float MaxCooling = 12f;

    /// <summary>
    /// The target being cooled. Updated when the cooling unit is equipped/unequipped.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? CoolingTarget;

}
