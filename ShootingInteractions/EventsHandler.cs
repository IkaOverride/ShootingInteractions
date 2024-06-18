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
using Exiled.CustomItems.API.Features;
using InventorySystem.Items.Pickups;
using InventorySystem.Items;
using InventorySystem;
using Mirror;

namespace ShootingInteractions
{
    internal sealed class EventsHandler
    {
        /// <summary>
        /// The plugin's config
        /// </summary>
        private static Config Config => Plugin.Instance.Config;

        /// <summary>
        /// A list of gameobjects that cannot be interacted with.
        /// </summary>
        public static List<GameObject> BlacklistedObjects = new();

        public void OnShooting(ShootingEventArgs args)
        {
            if (Config.AccurateBullets)
                return;

            // Check what's the player shooting at with a raycast, and return if the raycast doesn't hit something within 70 distance (maximum realistic distance)
            if (!Physics.Raycast(args.Player.CameraTransform.position, args.Player.CameraTransform.forward, out RaycastHit raycastHit, 70f, ~(1 << 1 | 1 << 13 | 1 << 16 | 1 << 28)))
                return;

            // Interact and check the interaction with the player that's shooting, and the GameObject associated to the raycast
            if (Interact(args.Player, raycastHit.transform.gameObject))
            {
                // Add the GameObject in the blacklist for a server tick
                BlacklistedObjects.Add(raycastHit.transform.gameObject);
                Timing.CallDelayed(0.02f, () => BlacklistedObjects.Remove(raycastHit.transform.gameObject));
            }
        }

        /// <summary>
        /// The shot event. Used for accurate shooting interaction.
        /// </summary>
        /// <param name="args">The <see cref="ShotEventArgs"/>.</param>
        public void OnShot(ShotEventArgs args)
        {
            if (!Config.AccurateBullets)
                return;

            // Check what's the player shooting at with a raycast, and return if the raycast doesn't hit something within 70 distance (maximum realistic distance)
            if (!Physics.Raycast(args.Player.CameraTransform.position, (args.RaycastHit.point - args.Player.CameraTransform.position).normalized, out RaycastHit raycastHit, 70f, ~(1 << 1 | 1 << 13 | 1 << 16 | 1 << 28)))
                return;

            // Interact and check the interaction with the player that's shooting, and the GameObject associated to the raycast
            if (Interact(args.Player, raycastHit.transform.gameObject))
            {
                // Add the GameObject in the blacklist for a server tick
                BlacklistedObjects.Add(raycastHit.transform.gameObject);
                Timing.CallDelayed(0.02f, () => BlacklistedObjects.Remove(raycastHit.transform.gameObject));
            }
        }

        /// <summary>
        /// Interact with the game object.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> that's doing the interaction</param>
        /// <param name="gameObject">The <see cref="GameObject"/></param>
        /// <returns>If the GameObject was an interactable object.</returns>
        public static bool Interact(Player player, GameObject gameObject)
        {
            BlacklistedObjects ??= new();

            // Return if the GameObject is in the blacklist
            if (BlacklistedObjects.Contains(gameObject))
            {
                Log.Debug("Player shot on a blacklisted GameObject: " + gameObject.name);
                return false;
            }

            // Doors
            if (gameObject.GetComponentInParent<RegularDoorButton>() is RegularDoorButton button)
            {
                // Get the door associated to the button
                Door door = Door.Get(button.GetComponentInParent<DoorVariant>());

                // Return if there's no door associated to the button, the door is moving, if it's locked and the player doesn't have bypass mode, or if it's an open checkpoint
                if (door is null || door.IsMoving || (door.IsLocked && !player.IsBypassModeEnabled) || (door.IsCheckpoint && door.IsOpen))
                    return true;

                // Set the cooldown and door interaction variables according to the type of door
                float cooldown = 0f;
                DoorInteraction doorInteraction = Config.Doors;
                
                if (door is CheckpointDoor checkpoint)
                {
                    cooldown = checkpoint.Base._openingTime + checkpoint.WaitTime + checkpoint.WarningTime;
                    doorInteraction = Config.Checkpoints;
                }
                else if (door is BasicDoor basicDoor)
                {
                    // Return if the door is doing its animation
                    if (basicDoor.RemainingCooldown >= 0.1f)
                        return true;

                    cooldown = basicDoor.Cooldown - 0.35f;

                    if (door.IsGate) {

                        // A gate takes less time to open than close
                        if (!door.IsOpen)
                            cooldown -= 0.35f;

                        doorInteraction = Config.Gates;
                    }
                }

                // Return if the interaction isn't enabled
                if (!doorInteraction.IsEnabled)
                    return true;

                // Should the buttons break ? (Generate number from 1 to 100 then check if lesser than interaction percentage)
                bool doorBreak = Random.Range(1, 101) <= doorInteraction.ButtonsBreakChance;

                // Lock the door if the door isn't locked and the buttons should break BEFORE moving, then unlock after the time indicated in the config, if it's greater than 0
                if (!door.IsLocked && doorBreak && !doorInteraction.MoveBeforeBreaking)
                {
                    door.ChangeLock(DoorLockType.SpecialDoorFeature);

                    if (doorInteraction.ButtonsBreakTime > 0)
                        Timing.CallDelayed(doorInteraction.ButtonsBreakTime, () => door.ChangeLock(DoorLockType.None));

                    // Don't interact with the door if the player doesn't have bypass mode enabled
                    if (!player.IsBypassModeEnabled)
                        return true;
                }

                // Deny access if the door is a keycard door, the player doesn't have bypass mode enabled, and either remote keycard is disabled or the player doesn't have a valid keycard in their inventory
                if (door.IsKeycardDoor && !player.IsBypassModeEnabled && (!doorInteraction.RemoteKeycard || !player.Items.Any(item => item is Keycard keycard && (keycard.Base.Permissions & door.RequiredPermissions.RequiredPermissions) != 0)))
                {
                    door.PlaySound(DoorBeepType.PermissionDenied);
                    return true;
                }

                // Open or close the door
                door.IsOpen = !door.IsOpen;

                // Lock the door if the door isn't locked and the buttons should break AFTER moving, then unlock after the time indicated in the config, if it's greater than 0
                if (!door.IsLocked && doorBreak && doorInteraction.MoveBeforeBreaking)
                    Timing.CallDelayed(cooldown, () =>
                    {

                        door.ChangeLock(DoorLockType.SpecialDoorFeature);

                        if (doorInteraction.ButtonsBreakTime > 0)
                            Timing.CallDelayed(doorInteraction.ButtonsBreakTime, () => door.ChangeLock(DoorLockType.None));
                    });
            }

            // Lockers
            else if (gameObject.GetComponentInParent<Locker>() is Locker locker && gameObject.GetComponentInParent<LockerChamber>() is LockerChamber chamber)
            {
                // The right remote keycard interaction config according to the locker type
                bool remoteKeycard;

                // Bulletproof locker
                if (((gameObject.name == "Collider Keypad") || (gameObject.name == "Collider Door" && !Config.BulletproofLockers.OnlyKeypad)) && Config.BulletproofLockers.IsEnabled)
                    remoteKeycard = Config.BulletproofLockers.RemoteKeycard;

                // Weapon locker
                else if (gameObject.name == "Door" && Config.WeaponLockers.IsEnabled)
                    remoteKeycard = Config.WeaponLockers.RemoteKeycard;

                // Else, the specific locker interaction isn't enabled, return
                else
                    return true;

                // Return if the locker doesn't allow interaction
                if (!chamber.CanInteract)
                    return true;

                // Deny access if the player doesn't have bypass mode enabled, and either remote keycard is disabled or the player doesn't have the right keycard in their inventory
                if (!player.IsBypassModeEnabled && (!remoteKeycard || !player.Items.Any(item => item is Keycard keycard && keycard.Base.Permissions.HasFlag(chamber.RequiredPermissions))))
                {
                    locker.RpcPlayDenied((byte) locker.Chambers.ToList().IndexOf(chamber));
                    return true;
                }

                // Open the locker
                chamber.SetDoor(!chamber.IsOpen, locker._grantedBeep);
                locker.RefreshOpenedSyncvar();
            }

            // Elevators
            else if (gameObject.GetComponentInParent<ElevatorPanel>() is ElevatorPanel panel && Config.Elevators.IsEnabled)
            {
                // Get the elevator associated to the button
                Lift elevator = Lift.Get(panel.AssignedChamber);

                // Return if there's no elevator associated to the button, it's moving, it's locked and the player doesn't have bypass mode enabled, or it can't get its doors
                if (elevator is null || elevator.IsMoving || !elevator.IsOperative || (elevator.IsLocked && !player.IsBypassModeEnabled) || !ElevatorDoor.AllElevatorDoors.TryGetValue(panel.AssignedChamber.AssignedGroup, out List<ElevatorDoor> list))
                    return true;

                // Should the buttons break ? (Generate a number from 1 to 100 then check if it's lesser than config percentage)
                bool elevatorBreak = Random.Range(1, 101) <= Config.Elevators.ButtonsBreakChance;

                // Lock the elevator if the door isn't locked and the buttons should break BEFORE moving, then unlock after the time indicated in the config if it's greater than 0
                if (!elevator.IsLocked && elevatorBreak && !Config.Elevators.MoveBeforeBreaking)
                {
                    foreach (ElevatorDoor door in list)
                    {
                        door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
                        elevator.Base.RefreshLocks(elevator.Group, door);
                    }

                    if (Config.Elevators.ButtonsBreakTime > 0)
                        Timing.CallDelayed(Config.Elevators.ButtonsBreakTime, () => elevator.ChangeLock(DoorLockReason.None));

                    // Don't interact with the elevator if the player doesn't have bypass mode enabled
                    if (!player.IsBypassModeEnabled)
                        return true;
                }

                // Move the elevator to the next level
                int nextLevel = panel.AssignedChamber.CurrentLevel + 1;
                if (nextLevel >= list.Count)
                    nextLevel = 0;

                elevator.TryStart(nextLevel);

                // Lock the door if the door isn't locked and the buttons should break AFTER moving, then unlock after the time indicated in the config, if it's greater than 0
                if (!elevator.IsLocked && elevatorBreak && Config.Elevators.MoveBeforeBreaking)
                {
                    foreach (ElevatorDoor door in list)
                    {
                        door.ServerChangeLock(DoorLockReason.SpecialDoorFeature, true);
                        elevator.Base.RefreshLocks(elevator.Group, door);
                    }

                    if (Config.Elevators.ButtonsBreakTime > 0)
                        Timing.CallDelayed(Config.Elevators.ButtonsBreakTime + elevator.MoveTime, () => elevator.ChangeLock(DoorLockReason.None));
                }
            }

            // Grenades
            else if (gameObject.GetComponentInParent<TimedGrenadePickup>() is TimedGrenadePickup grenadePickup)
            {
                // Custom grenades
                if (CustomItem.TryGet(Pickup.Get(grenadePickup), out CustomItem customItem))
                {
                    // Return if custom grenades aren't enabled
                    if (!Config.CustomGrenades.IsEnabled)
                        return true;

                    // Set the attacker to the player shooting and explode the custom grenade
                    grenadePickup._attacker = player.Footprint;
                    grenadePickup._replaceNextFrame = true;
                }
                else
                {
                    TimedProjectileInteraction grenadeInteraction = grenadePickup.Info.ItemId switch
                    {
                        ItemType.GrenadeHE => Config.FragGrenades,
                        ItemType.GrenadeFlash => Config.Flashbangs,
                        _ => new TimedProjectileInteraction() { IsEnabled = false }
                    };

                    // Return if the interaction isn't enabled, it can't get the grenade base, or it can't get the throwable
                    if (!grenadeInteraction.IsEnabled || !InventoryItemLoader.AvailableItems.TryGetValue(grenadePickup.Info.ItemId, out ItemBase grenadeBase) || (grenadeBase is not ThrowableItem grenadeThrowable))
                        return true;

                    // Instantiate the projectile
                    ThrownProjectile grenadeProjectile = Object.Instantiate(grenadeThrowable.Projectile);

                    // Set the physics of the projectile
                    PickupStandardPhysics grenadeProjectilePhysics = grenadeProjectile.PhysicsModule as PickupStandardPhysics;
                    PickupStandardPhysics grenadePickupPhysics = grenadePickup.PhysicsModule as PickupStandardPhysics;
                    if (grenadeProjectilePhysics is not null && grenadePickupPhysics is not null)
                    {
                        Rigidbody grenadeProjectileRigidbody = grenadeProjectilePhysics.Rb;
                        Rigidbody grenadePickupRigidbody = grenadePickupPhysics.Rb;
                        grenadeProjectileRigidbody.position = grenadePickupRigidbody.position;
                        grenadeProjectileRigidbody.rotation = grenadePickupRigidbody.rotation;
                        grenadeProjectileRigidbody.velocity = grenadePickupRigidbody.velocity + (player.CameraTransform.forward * (grenadeInteraction.AdditionalVelocity ? grenadeInteraction.VelocityForce : 0));
                    }

                    // Lock the grenade pickup
                    grenadePickup.Info.Locked = true;

                    // Set the network info and owner of the projectile
                    grenadeProjectile.NetworkInfo = grenadePickup.Info;
                    grenadePickup._attacker = player.Footprint;
                    grenadeProjectile.PreviousOwner = player.Footprint;

                    // Spawn the grenade projectile
                    NetworkServer.Spawn(grenadeProjectile.gameObject);

                    // Should the grenade have a malfunction ? (Generate number from 1 to 100 then check if lesser than interaction percentage)
                    // Set the fuse time of the grenade projectile to the interaction fuse time if it should malfunction
                    if (Random.Range(1, 101) <= grenadeInteraction.MalfunctionChance)
                        (grenadeProjectile as TimeGrenade)._fuseTime = grenadeInteraction.MalfunctionFuseTime;

                    // Activate the projectile and destroy the pickup
                    grenadeProjectile.ServerActivate();
                    grenadePickup.DestroySelf();
                }
            }

            // SCP-2176
            else if (gameObject.GetComponentInParent<Scp2176Projectile>() is Scp2176Projectile projectile && Config.Scp2176.IsEnabled)
                projectile.ServerImmediatelyShatter();

            return false;
        }
    }
}
