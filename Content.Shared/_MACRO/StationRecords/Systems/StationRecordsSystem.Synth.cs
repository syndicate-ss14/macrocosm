using Content.Shared._CD.Traits;

namespace Content.Shared.StationRecords.Systems;

public sealed partial class StationRecordsSystem
{
    [Dependency] private EntityQuery<SynthComponent> _synthQuery = default!;
}
