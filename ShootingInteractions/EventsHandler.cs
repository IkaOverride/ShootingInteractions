using Exiled.API.Features;
using Exiled.API.Features.Items;
using Exiled.Events.EventArgs.Player;
using Interactables.Interobjects.DoorUtils;
using Interactables.Interobjects;
using MEC;
using System.Linq;
using UnityEngine;
using DoorBeepType = Exiled.API.Enums.DoorBeepType;
using DoorLockType = Exiled.API.Enums.DoorLockType;
using Random = System.Random;
using System.Collections.Generic;
using InventorySystem.Items.ThrowableProjectiles;
using Exiled.API.Features.Doors;
using ElevatorDoor = Interactables.Interobjects.ElevatorDoor;
using Exiled.API.Features.Pickups;

namespace ShootingInteractions {

    internal sealed class EventsHandler {
        public void OnShooting(ShootingEventArgs args) {

            // Check what's the player shooting at with a raycast
            Physics.Raycast(args.Player.CameraTransform.position, args.Player.CameraTransform.forward, out RaycastHit raycastHit, 70f, ~(1 << 13 | 1 << 16));

            // Return if the raycast doesn't hit anything (If the collider is null)
            if (raycastHit.collider is null) 
                return;

            // Get the GameObject associated to the raycast
            GameObject gameObject = raycastHit.transform.gameObject;

            // Get the plugin's config
            Config config = ShootingInteractions.Instance.Config;

            // Doors (If there's a RegularDoorButton in the GameObject)
            if (gameObject.GetComponentInParent<RegularDoorButton>() is RegularDoorButton button && config.Doors) {

                // Get the door associated to the button
                Door door = Door.Get(button.GetComponentInParent<DoorVariant>());

                // Return if there's no door associated to the button
                if (door is null)
                    return;

                // Return if the door is moving, if it's locked or if it's an open checkpoint
                if (door.IsMoving || door.IsLocked || (door.IsCheckpoint && door.IsOpen))
                    return;

                // Should the buttons break ? (Generate number 1 to 100 -> Check if lesser than config percentage)
                bool doorBreak = new Random().Next(1, 101) <= config.DoorButtonsBreakChance;

                Log.Debug("Someone shoot a door button while it could be interacted with. Should its buttons break : " + doorBreak);

                // If the buttons should break instantly
                if (doorBreak && !config.DoorMoveBeforeBreaking) {
                    // Lock the door
                    door.ChangeLock(DoorLockType.SpecialDoorFeature);

                    // Unlock the door after the time indicated in the config, if it's greater than 0
                    if (config.DoorButtonsBreakTime > 0f)
                        Timing.CallDelayed(config.DoorButtonsBreakTime, () => door.ChangeLock(DoorLockType.None));

                    return;
                }

                // Deny access if a keycard is required and either CheckKeycard config is set to false or the player doesn't have the right keycard to open it
                if (door.RequiredPermissions.RequiredPermissions != KeycardPermissions.None && !args.Player.IsBypassModeEnabled && (config.DoorCheckKeycard == false || !args.Player.Items.Any(item => item is Keycard keycard && (keycard.Base.Permissions & door.RequiredPermissions.RequiredPermissions) != 0))) {
                    door.PlaySound(DoorBeepType.PermissionDenied);
                    return;
                }

                // Open or close the door
                door.IsOpen = !door.IsOpen;

                // If the buttons should break and should break after the door moved
                if (doorBreak && config.DoorMoveBeforeBreaking) {

                    // Wait time, to lock only after the animation is done
                    float additionalWait = 0f;

                    // Wait time for a checkpoint door
                    if (door.IsCheckpoint)
                        additionalWait = 7.5f;

                    // Wait time for a gate door
                    else if (door.IsGate)
                        additionalWait = 2.25f;

                    // Wait time for a normal door
                    else
                        additionalWait = 1f;

                    // Lock the door
                    Timing.CallDelayed(additionalWait, () => door.ChangeLock(DoorLockType.SpecialDoorFeature));

                    // Unlock the buttons after the time indicated in the config, if it's greater than 0
                    if (config.DoorButtonsBreakTime > 0f) {
                        Timing.CallDelayed(config.DoorButtonsBreakTime + additionalWait, () => door.ChangeLock(DoorLockType.None));
                    }
                }
            }

            // Elevators (If there's an ElevatorPanel in the GameObject)
            else if (gameObject.GetComponentInParent<ElevatorPanel>() is ElevatorPanel panel && config.Elevators) {

                // Get the elevator associated to the button
                Lift elevator = Lift.Get(panel.AssignedChamber);

                // Return if there's no elevator associated to the button
                if (elevator is null)
                    return;

                // Return if there elevator is moving, if it's locked or if it can't get a list of all elevator doors associated to the elevator
                if (elevator.IsMoving || !elevator.IsOperative || elevator.IsLocked || !ElevatorDoor.AllElevatorDoors.TryGetValue(panel.AssignedChamber.AssignedGroup, out List<ElevatorDoor> list))
                    return;

                // Should the buttons break ? (Generate number 1 to 100 -> Check if lesser than config percentage)
                bool elevatorBreak = new Random().Next(1, 101) <= config.ElevatorButtonsBreakChance;

                Log.Debug("Someone shoot an elevator button while it could be interacted with. Should its buttons break : " + elevatorBreak);

                // If the buttons should break and should break before moving
                if (elevatorBreak && !config.ElevatorMoveBeforeBreaking) {

                    // Lock the elevator doors
                    foreach (ElevatorDoor door in list) {
                        door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
                        elevator.Base.RefreshLocks(elevator.Group, door);
                    }

                    // Unlock the buttons after the time indicated in the config, if it's greater than 0
                    if (config.ElevatorButtonsBreakTime > 0)
                        Timing.CallDelayed(config.ElevatorButtonsBreakTime, () => elevator.ChangeLock(DoorLockReason.None));

                    return;
                }

                // Compute the next level for the elevator
                int nextLevel = panel.AssignedChamber.CurrentLevel + 1;
                if (nextLevel >= list.Count)
                    nextLevel = 0;

                // Move the elevator to the next level
                elevator.TryStart(nextLevel);

                // If the buttons should break and should break after moving
                if (elevatorBreak && config.ElevatorMoveBeforeBreaking) {

                    // Lock the elevator doors
                    foreach (ElevatorDoor door in list) {
                        door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
                        elevator.Base.RefreshLocks(elevator.Group, door);
                    }

                    // Unlock the buttons after the time indicated in the config, if it's greater than 0
                    if (config.ElevatorButtonsBreakTime > 0f)
                        Timing.CallDelayed(config.ElevatorButtonsBreakTime + 6f, () => elevator.ChangeLock(DoorLockReason.None));
                }
            } 
            
            // Grenades (If there's a TimedGrenadePickup in the GameObject)
            else if (gameObject.GetComponentInParent<TimedGrenadePickup>() is TimedGrenadePickup pickupBase && config.Grenades) {

                // Get the GrenadePickup associated to the timed one
                GrenadePickup pickup = Pickup.Get(pickupBase).As<GrenadePickup>();

                // Explode and mark the player that is shooting as the attacker
                pickup.Explode(args.Player.Footprint);
            }
        }
    }
}
