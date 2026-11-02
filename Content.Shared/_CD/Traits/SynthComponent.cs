using Robust.Shared.GameStates; // MACRO - Move synthetic trait to shared

namespace Content.Shared._CD.Traits; // MACRO - Move synthetic trait to shared

/// <summary>
/// Set players' blood to coolant
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SynthSystem))] // MACRO - Move synthetic trait to shared
public sealed partial class SynthComponent : Component { } // MACRO - Refactor notification to own component
