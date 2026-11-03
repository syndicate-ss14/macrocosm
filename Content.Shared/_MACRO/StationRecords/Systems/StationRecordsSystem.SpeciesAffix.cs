using Content.Shared._MACRO.Traits;

namespace Content.Shared.StationRecords.Systems;

public sealed partial class StationRecordsSystem
{
    [Dependency] private EntityQuery<SpeciesAffixComponent> _affixQuery = default!;
}
