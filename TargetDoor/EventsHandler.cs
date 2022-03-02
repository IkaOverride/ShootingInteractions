using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Interactables.Interobjects.DoorUtils;
using UnityEngine;
using DoorBeepType = Exiled.API.Enums.DoorBeepType;

namespace TargetDoor {
    internal sealed class EventsHandler {
        public void OnShooting(ShootingEventArgs args) {
            Physics.Raycast(args.Shooter.CameraTransform.position, args.Shooter.CameraTransform.forward, out RaycastHit raycastHit, 70);

            GameObject gameObject = raycastHit.transform.gameObject;

            if (gameObject.GetComponentInParent<RegularDoorButton>() is RegularDoorButton button) {
                Door door = Door.Get(button.GetComponentInParent<DoorVariant>());

                if (door.Base._collidersActivationPending) return;
                if (door.IsLocked || door.RequiredPermissions.RequiredPermissions != KeycardPermissions.None) {
                    door.PlaySound(DoorBeepType.PermissionDenied);
                    return;
                }

                door.IsOpen = !door.IsOpen;
            }
        }
    }
}
