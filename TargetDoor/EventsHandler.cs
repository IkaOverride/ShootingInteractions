using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Interactables.Interobjects.DoorUtils;
using MEC;
using System.Linq;
using UnityEngine;
using DoorBeepType = Exiled.API.Enums.DoorBeepType;
using DoorLockType = Exiled.API.Enums.DoorLockType;
using Random = System.Random;

namespace TargetDoor {

    internal sealed class EventsHandler {
        public void OnShooting(ShootingEventArgs args) {
            // Check what's the player shooting at with a raycast
            Physics.Raycast(args.Player.CameraTransform.position, args.Player.CameraTransform.forward, out RaycastHit raycastHit, 70f, ~(1 << 13));
            if (raycastHit.collider is null) return;
            GameObject gameObject = raycastHit.transform.gameObject;
            // Check if the player is shooting on a regular door button
            if (gameObject.GetComponentInParent<RegularDoorButton>() is RegularDoorButton button) {
                // Get the door associated to the button
                Door door = Door.Get(button.GetComponentInParent<DoorVariant>());

                // Return if there's no door associated to the button
                if (door is null) 
                    return;

                // Return if the door is moving
                if (door.IsMoving)
                    return;

                Config config = TargetDoor.Instance.Config;

                // Deny access if the door is locked. If a keycard is required, deny if it shouldn't check player keycards or if the player doesn't have the keycard to open it
                if (door.IsLocked || (door.RequiredPermissions.RequiredPermissions != KeycardPermissions.None && (config.CheckKeycard == false || !args.Player.Items.Any(item => item is Keycard keycard && (keycard.Base.Permissions & door.RequiredPermissions.RequiredPermissions) != 0)))) {
                    door.PlaySound(DoorBeepType.PermissionDenied);
                    return;
                }

                // Should the door break ? (Generate number 1 to 100 -> Check if lesser than config percentage)
                bool doorBreak = new Random().Next(1, 101) <= config.DoorBreakChance;

                if (config.Debug)
                    Log.Debug("Someone shoot a button. Door break : " + doorBreak);

                // Open the door if it shouldn't break or if it should break AFTER the animation
                if (!doorBreak || !config.DoorBreakBefore)
                    door.IsOpen = !door.IsOpen;

                // Lock the door if it should break (And unlock it after the time indicated in the config it it's greater than 0)
                if (doorBreak) {
                    door.ChangeLock(DoorLockType.SpecialDoorFeature);
                    if (config.DoorBreakTime > 0)
                        Timing.CallDelayed(config.DoorBreakTime, () => door.ChangeLock(DoorLockType.None));
                }
            }
        }
    }
}
