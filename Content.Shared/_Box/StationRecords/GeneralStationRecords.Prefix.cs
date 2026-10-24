namespace Content.Shared.StationRecords;

public sealed partial record GeneralStationRecord
{
    /// <summary>
    /// Does the species have a prefix, and if so what is it?
    /// </summary>
    [DataField]
    public string? SpeciesAffix;
}
