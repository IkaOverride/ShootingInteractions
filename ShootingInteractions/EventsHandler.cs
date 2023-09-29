using Exiled.API.Features;
using Exiled.API.Features.Doors;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups;
using Exiled.Events.EventArgs.Player;
using Interactables.Interobjects.DoorUtils;
using Interactables.Interobjects;
using InventorySystem.Items.ThrowableProjectiles;
using MapGeneration.Distributors;
using MEC;
using ShootingInteractions.Configs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DoorBeepType = Exiled.API.Enums.DoorBeepType;
using DoorLockType = Exiled.API.Enums.DoorLockType;
using BasicDoor = Exiled.API.Features.Doors.BasicDoor;
using CheckpointDoor = Exiled.API.Features.Doors.CheckpointDoor;
using ElevatorDoor = Interactables.Interobjects.ElevatorDoor;
using Scp2176Projectile = InventorySystem.Items.ThrowableProjectiles.Scp2176Projectile;

namespace ShootingInteractions {

    internal sealed class EventsHandler {

        /// <summary>
        /// The plugin's config
        /// </summary>
        private static readonly Config Config = ShootingInteractions.Instance.Config;

        /// <summary>
        /// The shooting event.
        /// </summary>
        /// <param name="args">The <see cref="ShootingEventArgs"/>.</param>
        public void OnShooting(ShootingEventArgs args) {

            // Check what's the player shooting at with a raycast, and return if the raycast doesn't hit something within 70 distance (maximum realistic distance)
            if (!Physics.Raycast(args.Player.CameraTransform.position, args.Player.CameraTransform.forward, out RaycastHit raycastHit, 70f, ~(1 << 13 | 1 << 16)))
                return;

            // Get the GameObject associated to the raycast
            GameObject gameObject = raycastHit.transform.gameObject;

            // Doors (If there's a RegularDoorButton in the GameObject)
            if (gameObject.GetComponentInParent<RegularDoorButton>() is RegularDoorButton button) {

                // Get the door associated to the button
                Door door = Door.Get(button.GetComponentInParent<DoorVariant>());

                // Return if there's no door associated to the button
                if (door is null)
                    return;

                // Return if the door is moving, if it's locked and the player doesn't have bypass mode, or if it's an open checkpoint
                if (door.IsMoving || (door.IsLocked && !args.Player.IsBypassModeEnabled) || (door.IsCheckpoint && door.IsOpen))
                    return;

                // Cooldown, to lock only after the animation is done
                float cooldown = 0f;

                // The right door interaction according to the door type
                DoorInteraction doorInteraction = Config.Doors;

                // If the door is a checkpoint door
                if (door is CheckpointDoor checkpoint) {

                    // Set the cooldown
                    cooldown = checkpoint.Base._openingTime + checkpoint.WaitTime + checkpoint.WarningTime;

                    // Checkpoint interaction
                    doorInteraction = Config.Checkpoints;
                }

                // If the door is a basic door
                else if (door is BasicDoor basicDoor) {

                    // Return if the door is doing its animation
                    if (basicDoor.RemainingCooldown >= 0.1f)
                        return;

                    // Set the cooldown
                    cooldown = basicDoor.Cooldown - 0.3f;

                    // If the door is a gate
                    if (door.IsGate) {

                        // Remove some time to the cooldown if the door is a closed gate (Gate takes less time to open in this case)
                        if (!door.IsOpen)
                            cooldown -= 0.35f;

                        // Gate interaction
                        doorInteraction = Config.Gates;
                    }
                }

                // Return if the door interaction isn't enabled
                if (!doorInteraction.IsEnabled)
                    return;

                // Should the buttons break ? (Generate number 1 to 100 -> Check if lesser than config percentage)
                bool doorBreak = Random.Range(1, 101) <= doorInteraction.ButtonsBreakChance;

                // If the buttons should break and should break before moving
                if (doorBreak && !doorInteraction.MoveBeforeBreaking) {

                    // Lock the door
                    door.ChangeLock(DoorLockType.SpecialDoorFeature);

                    // Unlock the door after the time indicated in the config, if it's greater than 0
                    if (doorInteraction.ButtonsBreakTime > 0f)
                        Timing.CallDelayed(doorInteraction.ButtonsBreakTime, () => door.ChangeLock(DoorLockType.None));

                    return;
                }

                // Deny access if the door is a keycard door, the player doesn't have bypass mode enabled, and either remote keycard is disabled or the player doesn't have a valid keycard in their inventory
                if (door.IsKeycardDoor && !args.Player.IsBypassModeEnabled && (!doorInteraction.RemoteKeycard || !args.Player.Items.Any(item => item is Keycard keycard && (keycard.Base.Permissions & door.RequiredPermissions.RequiredPermissions) != 0))) {
                    door.PlaySound(DoorBeepType.PermissionDenied);
                    return;
                }

                // Open or close the door
                door.IsOpen = !door.IsOpen;

                // If the buttons should break and should break after moving
                if (doorBreak && doorInteraction.MoveBeforeBreaking)
                    Timing.CallDelayed(cooldown, () => {

                        // Lock the door
                        door.ChangeLock(DoorLockType.SpecialDoorFeature);

                        // Unlock the buttons after the time indicated in the config, if it's greater than 0
                        if (doorInteraction.ButtonsBreakTime > 0f)
                            Timing.CallDelayed(doorInteraction.ButtonsBreakTime, () => door.ChangeLock(DoorLockType.None));
                    });
            }

            // Bulletproof lockers
            else if (gameObject.name == "Collider Keypad" && gameObject.GetComponentInParent<Locker>() is Locker locker && gameObject.GetComponentInParent<LockerChamber>() is LockerChamber chamber && Config.BulletproofLockers.IsEnabled) {

                // Return if the chamber doesn't allow interaction
                if (!chamber.CanInteract)
                    return;

                // Deny access if the player doesn't have bypass mode enabled, and either remote keycard is disabled or the player doesn't have the right keycard in their inventory
                if (!args.Player.IsBypassModeEnabled && (!Config.BulletproofLockers.RemoteKeycard || !args.Player.Items.Any(item => item is Keycard keycard && keycard.Base.Permissions.HasFlag(chamber.RequiredPermissions)))) {
                    locker.RpcPlayDenied((byte) locker.Chambers.ToList().IndexOf(chamber));
                    return;
                }

                // Open the locker chamber
                chamber.SetDoor(!chamber.IsOpen, locker._grantedBeep);

                // Refresh opened syncvar
                locker.RefreshOpenedSyncvar();              
            }

            // Elevators (If there's an ElevatorPanel in the GameObject)
            else if (gameObject.GetComponentInParent<ElevatorPanel>() is ElevatorPanel panel && Config.Elevators.IsEnabled) {

                // Get the elevator associated to the button
                Lift elevator = Lift.Get(panel.AssignedChamber);

                // Return if there's no elevator associated to the button
                if (elevator is null)
                    return;

                // Return if there elevator is moving, if it's locked and the player doesn't have bypass mode enabled, or if it can't get a list of all elevator doors associated to the elevator
                if (elevator.IsMoving || !elevator.IsOperative || (elevator.IsLocked && !args.Player.IsBypassModeEnabled) || !ElevatorDoor.AllElevatorDoors.TryGetValue(panel.AssignedChamber.AssignedGroup, out List<ElevatorDoor> list))
                    return;

                // Should the buttons break ? (Generate number 1 to 100 -> Check if lesser than config percentage)
                bool elevatorBreak = Random.Range(1, 101) <= Config.Elevators.ButtonsBreakChance;

                // If the buttons should break and should break before moving
                if (elevatorBreak && !Config.Elevators.MoveBeforeBreaking) {

                    // Lock the elevator doors
                    foreach (ElevatorDoor door in list) {
                        door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
                        elevator.Base.RefreshLocks(elevator.Group, door);
                    }

                    // Unlock the buttons after the time indicated in the config, if it's greater than 0
                    if (Config.Elevators.ButtonsBreakTime > 0)
                        Timing.CallDelayed(Config.Elevators.ButtonsBreakTime, () => elevator.ChangeLock(DoorLockReason.None));

                    return;
                }

                // Compute the next level for the elevator
                int nextLevel = panel.AssignedChamber.CurrentLevel + 1;
                if (nextLevel >= list.Count)
                    nextLevel = 0;

                // Move the elevator to the next level
                elevator.TryStart(nextLevel);

                // If the buttons should break and should break after moving
                if (elevatorBreak && Config.Elevators.MoveBeforeBreaking) {

                    // Lock the elevator doors
                    foreach (ElevatorDoor door in list) {
                        door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
                        elevator.Base.RefreshLocks(elevator.Group, door);
                    }

                    // Unlock the buttons after the time indicated in the config, if it's greater than 0
                    if (Config.Elevators.ButtonsBreakTime > 0f)
                        Timing.CallDelayed(Config.Elevators.ButtonsBreakTime + (elevator.AnimationTime + elevator.Base._rotationTime + elevator.Base._doorOpenTime + elevator.Base._doorCloseTime), () => elevator.ChangeLock(DoorLockReason.None));
                }
            }

            // Grenades (If there's a TimedGrenadePickup in the GameObject)
            else if (gameObject.GetComponentInParent<TimedGrenadePickup>() is TimedGrenadePickup pickup) {

                // Create a new grenade
                Item item = Item.Create(Pickup.Get(pickup).Type);

                // If the grenade is a frag grenade and it is enabled in the config
                if (item is ExplosiveGrenade explosiveGrenade && Config.FragGrenades.IsEnabled) {

                    // Explode the frag grenade instantly if it is set in the config
                    if (Config.FragGrenades.ExplodeInstantly)
                        explosiveGrenade.FuseTime = 0.1f;

                    // Spawn the frag grenade to the position of the pickup with the player as the owner and activate it
                    explosiveGrenade.SpawnActive(pickup.Position, args.Player);

                    // Destroy the pickup
                    Object.Destroy(pickup.gameObject);
                }

                // If the grenade is a flashbang and it is enabled in config
                else if (item is FlashGrenade flashGrenade && Config.Flashbangs.IsEnabled) {

                    // Explode the flashbang instantly if it is set in the config
                    if (Config.Flashbangs.ExplodeInstantly)
                        flashGrenade.FuseTime = 0.1f;

                    // Spawn the flashbang to the position of the pickup with the player as the owner and activate it
                    flashGrenade.SpawnActive(pickup.Position, args.Player);

                    // Destroy the pickup
                    Object.Destroy(pickup.gameObject);
                }

                // If the grenade isn't enabled in the config, destroy the newly created grenade
                else
                    Object.Destroy(item.Base.gameObject);
            }

            // SCP-2176 (If there's a Scp2176Projectile in the GameObject): immediately shatter the SCP-2176
            else if (gameObject.GetComponentInParent<Scp2176Projectile>() is Scp2176Projectile projectile && Config.Scp2176.IsEnabled)
                projectile.ServerImmediatelyShatter();
        }
    }
}
