using Content.Shared.IdentityManagement;
using Content.Shared.Popups;
using Robust.Shared.Random;

namespace Content.Shared.EntityEffects.Effects.Transform;

public sealed partial class PopupMessageEntityEffectSystem
{
    /// <summary>
    ///     Spawns a random popup message on the given entity with the given parameters.
    /// </summary>
    /// <param name="entity">The entity to spawn a popup message on.</param>
    /// <param name="messages">An array of possible random messages.</param>
    /// <param name="popupType">The visual type of the popup.</param>
    /// <param name="method">The popup API type to use.</param>
    /// <param name="recipients">Whether this popup only shows for the entity, or for everyone.</param>
    /// <remarks>
    ///     This was extracted from the body of the <see cref="Effect"/> function.
    /// </remarks>
    public void PopupMessage(EntityUid entity,
        string[] messages,
        PopupType popupType,
        PopupMethod method,
        PopupRecipients recipients)
    {
        // TODO: When we get proper random prediction remove this check.
        if (_net.IsClient)
            return;

        var entityId = Identity.Entity(entity, EntityManager);
        var msg = Loc.GetString(_random.Pick(messages), ("entity", entityId));

        switch ((method, recipients))
        {
            case (PopupMethod.PopupEntity, PopupRecipients.Local):
                _popup.PopupEntity(msg, entity, entity, popupType);
                break;
            case (PopupMethod.PopupEntity, PopupRecipients.Pvs):
                _popup.PopupEntity(msg, entity, popupType);
                break;
            case (PopupMethod.PopupCoordinates, PopupRecipients.Local):
                _popup.PopupCoordinates(msg, Transform(entity).Coordinates, entity, popupType);
                break;
            case (PopupMethod.PopupCoordinates, PopupRecipients.Pvs):
                var key = GetNetEntity(entity).Id;
                _popup.PopupCoordinates(msg, Transform(entity).Coordinates, popupType, predictionKey: key);
                break;
        }
    }
}
