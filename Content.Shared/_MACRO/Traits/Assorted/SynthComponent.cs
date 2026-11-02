// MACRO - File originally from https://github.com/cosmatic-drift-14/cosmatic-drift/pull/149, heavily modified by ZigTag for various forks, and consolidated here.
// Permalink: https://github.com/cosmatic-drift-14/cosmatic-drift/blob/4959877f52a4b0e3620a813c4dd5e26a273aff88/Content.Server/_CD/Traits/SynthComponent.cs

using Robust.Shared.GameStates; // MACRO - Move synthetic trait to shared

namespace Content.Shared._MACRO.Traits; // MACRO - Move synthetic trait to shared

/// <summary>
/// Set players' blood to coolant
/// </summary>
[RegisterComponent, NetworkedComponent, Access(typeof(SynthSystem))] // MACRO - Move synthetic trait to shared
public sealed partial class SynthComponent : Component { } // MACRO - Refactor notification to own component
