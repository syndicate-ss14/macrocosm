using Content.Shared.Objectives;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Content.Shared._Starlight.CollectiveMind; // Starlight - Collective Minds

using Robust.Shared.Serialization;

namespace Content.Shared.CharacterInfo;

[Serializable, NetSerializable]
public sealed class RequestCharacterInfoEvent : EntityEventArgs
{
    public readonly NetEntity NetEntity;

    public RequestCharacterInfoEvent(NetEntity netEntity)
    {
        NetEntity = netEntity;
    }
}

[Serializable, NetSerializable]
public sealed class CharacterInfoEvent : EntityEventArgs
{
    public readonly NetEntity NetEntity;
    public readonly ProtoId<JobPrototype>? Job;
    public readonly Dictionary<string, List<ObjectiveInfo>> Objectives;
    public readonly string? Briefing;
    public readonly Dictionary<CollectiveMindPrototype, CollectiveMindMemberData>? CollectiveMinds; // Starlight - Collective Minds

    public CharacterInfoEvent(NetEntity netEntity,
        ProtoId<JobPrototype>? job,
        Dictionary<string, List<ObjectiveInfo>> objectives,
        string? briefing,
        Dictionary<CollectiveMindPrototype, CollectiveMindMemberData>? collectiveMinds) // Starlight - Collective Minds
    {
        NetEntity = netEntity;
        Objectives = objectives;
        Briefing = briefing;
        Job = job;
        CollectiveMinds = collectiveMinds; // Starlight - Collective Minds
    }
}
