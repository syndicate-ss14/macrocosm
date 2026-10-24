using Content.Shared._Box.Traits;

namespace Content.Shared.Humanoid;

public sealed partial class HumanoidProfileSystem
{
    public string GetAffixRepresentation(EntityUid uid, string speciesText)
    {
        return TryComp<SpeciesAffixComponent>(uid, out var affix)
            ? Loc.GetString(affix.ExamineAffix, ("species", speciesText))
            : speciesText;
    }
}
