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
using DoorLockType = Exiled.API.Enums.DoorLockType;
using BasicDoor = Exiled.API.Features.Doors.BasicDoor;
using CheckpointDoor = Exiled.API.Features.Doors.CheckpointDoor;
using ElevatorDoor = Interactables.Interobjects.ElevatorDoor;
using Scp2176Projectile = InventorySystem.Items.ThrowableProjectiles.Scp2176Projectile;
using InventorySystem.Items.Pickups;
using InventorySystem.Items;
using InventorySystem;
using Mirror;
using Interactables.Interobjects.DoorButtons;
using System.Diagnostics.Eventing.Reader;
using Interactables;
using YamlDotNet.Serialization;
using Exiled.API.Features.DamageHandlers;
using PlayerStatsSystem;

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

        /// <summary>
        /// The shot event. Used for accurate shooting interaction.
        /// </summary>
        /// <param name="args">The <see cref="ShotEventArgs"/>.</param>
        public void OnShot(ShotEventArgs args)
        {
            // Check what's the player shooting at with a raycast, and return if the raycast doesn't hit something within 70 distance (maximum realistic distance)
            // Layer 1 = VolumeOverrideTunne
            // Layer 13 = Player's Hitboxes
            // Layer 28 = Broken Glasses
            // Layer 29 = Fences

            Vector3 origin = args.Player.CameraTransform.position;
            Vector3 direction = Config.AccurateBullets ? (args.RaycastHit.point - origin).normalized : args.Player.CameraTransform.forward;

            if (!Physics.Raycast(origin, direction, out RaycastHit raycastHit, 70f, ~(1 << 1 | 1 << 13 | 1 << 28 | 1 << 29)))
                return;

            // Interact if the object isn't in the blacklist
            if (!BlacklistedObjects.Contains(raycastHit.transform.gameObject) && Interact(args.Player, raycastHit.transform.gameObject, args.Firearm, direction))
            {
                // Add the GameObject in the blacklist for a server tick
                BlacklistedObjects.Add(raycastHit.transform.gameObject);
                Timing.CallDelayed(Time.smoothDeltaTime, () => BlacklistedObjects.Remove(raycastHit.transform.gameObject));
            }
        }


        public void OnHurt(HurtEventArgs ev)
        {
            PlayerStatsSystem.DamageHandlerBase suicidio = new CustomReasonDamageHandler(":trollface:");
            DamageHandler damage = new DamageHandler(ev.Player, suicidio);
            ev.Player.Kill(damage);
        }

        /// <summary>
        /// Interact with the game object.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> that's doing the interaction</param>
        /// <param name="gameObject">The <see cref="GameObject"/></param>
        /// <returns>If the GameObject was an interactable object.</returns>
        public static bool Interact(Player player, GameObject gameObject, Firearm firearm, Vector3 direction)
        {
            Log.Info(gameObject.name);
            Log.Info(gameObject.layer);
            Log.Info(string.Join(", ", gameObject.GetComponents<Component>().ToList()));
            Log.Info(string.Join(", ", gameObject.GetComponentsInParent<Component>().ToList()));
            Log.Info(string.Join(", ", gameObject.GetComponentsInChildren<Component>().ToList()));

            // Doors
            if (gameObject.GetComponentInParent<BasicDoorButton>() is BasicDoorButton button)
            {
                // Get the door associated to the button
                Door door = Door.Get(button.GetComponentInParent<DoorVariant>());

                // Return if:
                //  - door can't be found
                //  - door is moving
                //  - door is locked, and bypass mode is disabled
                //  - it's an open checkpoint
                if (door is null || door.IsMoving || (door.IsLocked && !player.IsBypassModeEnabled) || (door.IsCheckpoint && door.IsOpen))
                    return true;

                // Get the door cooldown (used to lock the door AFTER it moved) and the config depending on the door type
                float cooldown = 0f;
                DoorInteraction interactionConfig = Config.Doors;

                if (door is CheckpointDoor checkpoint)
                {
                    cooldown = 0.6f + checkpoint.WaitTime + checkpoint.WarningTime;
                    interactionConfig = Config.Checkpoints;
                }
                else if (door is BasicDoor interactableDoor)
                {
                    // Return if the door is in cooldown
                    if (interactableDoor.RemainingCooldown >= 0.1f)
                        return true;

                    cooldown = interactableDoor.Cooldown - 0.35f;

                    if (door.IsGate)
                    {
                        // A gate takes less time to open than close
                        if (!door.IsOpen)
                            cooldown -= 0.35f;

                        interactionConfig = Config.Gates;
                    }
                }

                // Return if the interaction isn't enabled
                if (!interactionConfig.IsEnabled)
                    return true;

                // Should the door get locked ? (Generate number from 1 to 100 then check if lesser than interaction percentage)
                bool shouldLock = !door.IsLocked && Random.Range(1, 101) <= interactionConfig.ButtonsBreakChance;

                // Lock the door if it should be locked BEFORE moving
                if (shouldLock && !interactionConfig.MoveBeforeBreaking)
                {
                    door.ChangeLock(DoorLockType.Isolation);

                    // Unlock the door after the time indicated in the config (if greater than 0)
                    if (interactionConfig.ButtonsBreakTime > 0)
                        Timing.CallDelayed(interactionConfig.ButtonsBreakTime, () => door.ChangeLock(DoorLockType.None));

                    // Don't interact if bypass mode is disabled
                    if (!player.IsBypassModeEnabled)
                        return true;
                }

                // Deny access if the door is a keycard door, bypass mode is disabled, and either: remote keycard is disabled OR the player has no keycard that open the door
                if (door.IsKeycardDoor && !player.IsBypassModeEnabled && (!interactionConfig.RemoteKeycard || !player.Items.Any(item => item is Keycard keycard && ((DoorPermissionFlags)keycard.Permissions & door.RequiredPermissions) != 0)))
                {
                    door.Base.PermissionsDenied(null, 0);
                    return true;
                }

                // Open or close the door
                door.IsOpen = !door.IsOpen;

                // Lock the door if it should be locked AFTER moving
                if (shouldLock && interactionConfig.MoveBeforeBreaking)
                    Timing.CallDelayed(cooldown, () =>
                    {
                        door.ChangeLock(DoorLockType.Isolation);

                        // Unlock the door after the time indicated in the config (if greater than 0)
                        if (interactionConfig.ButtonsBreakTime > 0)
                            Timing.CallDelayed(interactionConfig.ButtonsBreakTime, () => door.ChangeLock(DoorLockType.None));
                    });

                return true;
            }

            // Lockers (Bulletproof, weapon or Experimental)
            else if (gameObject.GetComponentInParent<Locker>() is Locker locker)
            {
                // The right remote keycard interaction config according to the locker type
                bool remoteKeycard;

                // Bulletproof locker
                if (((gameObject.name == "Collider Keypad") || (gameObject.name == "Collider Door" && !Config.BulletproofLockers.OnlyKeypad)) && Config.BulletproofLockers.IsEnabled)
                    remoteKeycard = Config.BulletproofLockers.RemoteKeycard;

                // Weapon locker
                else if (gameObject.name == "Door" && Config.WeaponLockers.IsEnabled)
                    remoteKeycard = Config.WeaponLockers.RemoteKeycard;

                // Experimental weapon locker
                else if (gameObject.name == "EWL_CenterDoor")
                    remoteKeycard = true;//Config.ExperimentalWeaponLockers.RemoteKeycard;

                else if (gameObject.name == "Collider Lid") //&& Config.LockerChambers.IsEnabled)
                    remoteKeycard = true;

                // Else, the specific locker interaction isn't enabled, return
                else
                    return true;

                if (gameObject.GetComponentInParent<ExperimentalWeaponLocker>() is ExperimentalWeaponLocker baseExpLocker)
                {
                    // Gets the wrapper for the experimental weapon locker
                    LabApi.Features.Wrappers.ExperimentalWeaponLocker expLocker = LabApi.Features.Wrappers.ExperimentalWeaponLocker.Get(baseExpLocker);

                    // Return if the locker doesn't allow interaction
                    if (!expLocker.CanInteract)
                        return true;

                    // Deny access if bypass mode is disabled and either: remote keycard is disabled OR the player has no keycard that open the locker
                    if (!player.IsBypassModeEnabled && (!remoteKeycard || !player.Items.Any(item => item is Keycard keycard && ((DoorPermissionFlags)keycard.Permissions).HasFlag(expLocker.RequiredPermissions))))
                    {
                        expLocker.PlayDeniedSound(expLocker.RequiredPermissions);
                        return true;
                    }

                    // Open the locker
                    expLocker.IsOpen = !expLocker.IsOpen;
                    locker.RefreshOpenedSyncvar();
                    return true;
                }

                if (gameObject.GetComponentInParent<LockerChamber>() is LockerChamber chamber)
                {
                    // Return if the locker doesn't allow interaction
                    if (!chamber.CanInteract)
                        return true;

                    // Deny access if bypass mode is disabled and either: remote keycard is disabled OR the player has no keycard that open the locker
                    if (!player.IsBypassModeEnabled && (!remoteKeycard || !player.Items.Any(item => item is Keycard keycard && ((DoorPermissionFlags)keycard.Permissions).HasFlag(chamber.RequiredPermissions))))
                    {
                        locker.RpcPlayDenied((byte)locker.Chambers.ToList().IndexOf(chamber), chamber.RequiredPermissions);
                        return true;
                    }

                    // Open the locker
                    chamber.SetDoor(!chamber.IsOpen, locker._grantedBeep);
                    locker.RefreshOpenedSyncvar();
                    return true;
                }



                return true;
            }

            // Elevators
            else if (gameObject.GetComponentInParent<ElevatorPanel>() is ElevatorPanel panel && Config.Elevators.IsEnabled)
            {
                // Return if the panel has no chamber
                if (panel.AssignedChamber is null)
                    return true;

                // Get the elevator associated to the button
                Lift elevator = Lift.Get(panel.AssignedChamber);

                // Return if:
                //  - elevator can't be found
                //  - elevator is moving
                //  - elevator is locked and bypass mode is disabled
                //  - no elevator doors
                if (elevator is null || elevator.IsMoving || !elevator.IsOperative || (elevator.IsLocked && !player.IsBypassModeEnabled) || !ElevatorDoor.AllElevatorDoors.TryGetValue(panel.AssignedChamber.AssignedGroup, out List<ElevatorDoor> list))
                    return true;

                // Should the elevator get locked ? (Generate a number from 1 to 100 then check if it's lesser than config percentage)
                bool shoudLock = !elevator.IsLocked && Random.Range(1, 101) <= Config.Elevators.ButtonsBreakChance;

                // Lock the door if it should be locked BEFORE moving
                if (shoudLock && !Config.Elevators.MoveBeforeBreaking)
                {
                    foreach (ElevatorDoor door in list)
                        door.ServerChangeLock(DoorLockReason.Isolation, true);

                    // Unlock the door after the time indicated in the config (if greater than 0)
                    if (Config.Elevators.ButtonsBreakTime > 0)
                        Timing.CallDelayed(Config.Elevators.ButtonsBreakTime, () =>
                        {
                            foreach (ElevatorDoor door in list)
                            {
                                door.NetworkActiveLocks = 0;
                                DoorEvents.TriggerAction(door, DoorAction.Unlocked, null);
                            }
                        });

                    // Don't interact if bypass mode is disabled
                    if (!player.IsBypassModeEnabled)
                        return true;
                }

                // Move the elevator to the next level
                elevator.TryStart(panel.AssignedChamber.NextLevel);

                // Lock the door if it should be locked AFTER moving
                if (shoudLock && Config.Elevators.MoveBeforeBreaking)
                {
                    foreach (ElevatorDoor door in list)
                        door.ServerChangeLock(DoorLockReason.Isolation, true);

                    // Unlock the door after the time indicated in the config (if greater than 0)
                    if (Config.Elevators.ButtonsBreakTime > 0)
                        Timing.CallDelayed(Config.Elevators.ButtonsBreakTime + elevator.MoveTime, () =>
                        {
                            foreach (ElevatorDoor door in list)
                            {
                                door.NetworkActiveLocks = 0;
                                DoorEvents.TriggerAction(door, DoorAction.Unlocked, null);
                            }
                        });
                }

                return true;
            }

            // Grenades
            else if (gameObject.GetComponentInParent<TimedGrenadePickup>() is TimedGrenadePickup grenadePickup)
            {
                // Custom grenades
                if (Plugin.GetCustomItem is not null && (bool)Plugin.GetCustomItem.Invoke(null, new[] { Pickup.Get(grenadePickup), null }))
                {
                    // Return if custom grenades aren't enabled
                    if (!Config.CustomGrenades.IsEnabled)
                        return true;

                    // Set the attacker to the player shooting and explode the custom grenade
                    grenadePickup.PreviousOwner = player.Footprint;
                    grenadePickup._replaceNextFrame = true;
                }

                // Non-custom grenades
                else
                {
                    TimedProjectileInteraction grenadeInteraction = grenadePickup.Info.ItemId switch
                    {
                        ItemType.GrenadeHE => Config.FragGrenades,
                        ItemType.GrenadeFlash => Config.Flashbangs,
                        _ => new TimedProjectileInteraction() { IsEnabled = false }
                    };

                    // Return if either: the interaction isn't enabled, it can't get the grenade base, or it can't get the throwable
                    if (!grenadeInteraction.IsEnabled || !InventoryItemLoader.AvailableItems.TryGetValue(grenadePickup.Info.ItemId, out ItemBase grenadeBase) || (grenadeBase is not ThrowableItem grenadeThrowable))
                    {
                        return true;
                    }

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
                    grenadePickup.PreviousOwner = player.Footprint;
                    grenadeProjectile.PreviousOwner = player.Footprint;

                    // Spawn the grenade projectile
                    NetworkServer.Spawn(grenadeProjectile.gameObject);

                    // Should the grenade have a custom fuse time ? (Generate number from 1 to 100 then check if lesser than interaction percentage)
                    if (Random.Range(1, 101) <= grenadeInteraction.MalfunctionChance)

                        // Set the custom fuse time
                        (grenadeProjectile as TimeGrenade)._fuseTime = Mathf.Max(Time.smoothDeltaTime * 2, grenadeInteraction.MalfunctionFuseTime);

                    // Activate the projectile and destroy the pickup
                    grenadeProjectile.ServerActivate();
                    grenadePickup.DestroySelf();
                }
            }

            else if (gameObject.GetComponentInParent<TimeGrenade>() is TimeGrenade scp018)
            {
                if (gameObject.name.Contains("HegProjectile"))
                    return true;

                // Custom SCP-018s
                if (Plugin.GetCustomItem is not null && (bool)Plugin.GetCustomItem.Invoke(null, new[] { Pickup.Get(scp018), null }))
                {
                    // Return if custom grenades aren't enabled
                    if (!Config.CustomGrenades.IsEnabled)
                        return true;

                    // Set the attacker to the player shooting and explode the custom grenade
                    scp018.PreviousOwner = player.Footprint;
                }

                // Non-custom SCP-018s
                else
                {
                    TimedProjectileInteraction grenadeInteraction = scp018.Info.ItemId switch
                    {
                        ItemType.SCP018 => Config.Scp018,
                        _ => new TimedProjectileInteraction() { IsEnabled = false }
                    };

                    // Return if either: the interaction isn't enabled, it can't get the grenade base, or it can't get the throwable
                    if (!grenadeInteraction.IsEnabled || !InventoryItemLoader.AvailableItems.TryGetValue(scp018.Info.ItemId, out ItemBase grenadeBase) || (grenadeBase is not ThrowableItem grenadeThrowable))
                    {
                        return true;
                    }

                    // Instantiate the projectile
                    ThrownProjectile grenadeProjectile = Object.Instantiate(grenadeThrowable.Projectile);

                    // Set the physics of the projectile
                    PickupStandardPhysics grenadeProjectilePhysics = grenadeProjectile.PhysicsModule as PickupStandardPhysics;
                    PickupStandardPhysics grenadePickupPhysics = scp018.PhysicsModule as PickupStandardPhysics;
                    if (grenadeProjectilePhysics is not null && grenadePickupPhysics is not null)
                    {
                        Rigidbody grenadeProjectileRigidbody = grenadeProjectilePhysics.Rb;
                        Rigidbody grenadePickupRigidbody = grenadePickupPhysics.Rb;
                        grenadeProjectileRigidbody.position = grenadePickupRigidbody.position;
                        grenadeProjectileRigidbody.rotation = grenadePickupRigidbody.rotation;
                        grenadeProjectileRigidbody.linearVelocity = direction * (firearm.Penetration * -30) - new Vector3(0, 0.5f, 0); //grenadePickupRigidbody.linearVelocity + (player.CameraTransform.forward * (grenadeInteraction.AdditionalVelocity ? grenadeInteraction.VelocityForce : 0));
                    }

                    // Lock the grenade pickup
                    scp018.Info.Locked = true;

                    // Set the network info and owner of the projectile
                    grenadeProjectile.NetworkInfo = scp018.Info;
                    scp018.PreviousOwner = player.Footprint;
                    grenadeProjectile.PreviousOwner = player.Footprint;

                    // Spawn the grenade projectile
                    NetworkServer.Spawn(grenadeProjectile.gameObject);

                    // Should the grenade have a custom fuse time ? (Generate number from 1 to 100 then check if lesser than interaction percentage)
                    if (Random.Range(1, 101) <= grenadeInteraction.MalfunctionChance)

                        // Set the custom fuse time
                        (grenadeProjectile as TimeGrenade)._fuseTime = Mathf.Max(Time.smoothDeltaTime * 2, grenadeInteraction.MalfunctionFuseTime);

                    // Activate the projectile and destroy the pickup
                    grenadeProjectile.ServerActivate();
                    scp018.DestroySelf();

                }
            }

            // SCP-2176
            else if (gameObject.GetComponentInParent<Scp2176Projectile>() is Scp2176Projectile projectile && Config.Scp2176.IsEnabled)
                projectile.ServerImmediatelyShatter();


            return false;
        }
    }
}
