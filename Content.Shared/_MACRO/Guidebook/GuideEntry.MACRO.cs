namespace Content.Shared.Guidebook;

public partial class GuideEntry
{
    /// <summary>
    ///     Determines whether or not this entry will show up in the "main" guidebook.
    /// </summary>
    /// </remarks>
    ///     Even if this is disabled, you can still make this guide entry accessible via book item,
    ///     UI button, etc. by passing it into the "guides" parameter in GuidebookUIController.OpenGuidebook().
    /// </remarks>
    [DataField]
    public bool ShowInGuidebook = true;

    /// <summary>
    ///     Determines whether or not this entry will show up when "includeChildren" is enabled
    ///     in GuidebookUIController.OpenGuidebook().
    /// </summary>
    /// <remarks>
    ///     This guide will still show up if included directly, including the "main" guidebook
    ///     with all entries if <see cref="ShowInGuidebook"/> is enabled.
    /// </remarks>
    [DataField]
    public bool IncludeAsChild = true;
}
