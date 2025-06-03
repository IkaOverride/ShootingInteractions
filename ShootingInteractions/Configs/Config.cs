using Exiled.API.Interfaces;
using System.ComponentModel;

namespace ShootingInteractions.Configs
{
    public sealed class Config : IConfig
    {
        [Description("Is the plugin enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Are the plugin's debug logs enabled")]
        public bool Debug { get; set; } = false;

        [Description("Check where the bullet actually lands instead of the center of the player's screen")]
        public bool AccurateBullets { get; set; } = false;

        [Description("Door buttons interaction")]
        public DoorInteraction Doors { get; set; } = new();

        [Description("Checkpoint door buttons interaction")]
        public DoorInteraction Checkpoints { get; set; } = new();

        [Description("Gate buttons interaction")]
        public DoorInteraction Gates { get; set; } = new();

        [Description("Weapon lockers interaction")]
        public LockerInteraction WeaponLockers { get; set; } = new();

        [Description("Bulletproof lockers interaction")]
        public BulletproofLockerInteraction BulletproofLockers { get; set; } = new();

        [Description("Elevators buttons interaction")]
        public ElevatorInteraction Elevators { get; set; } = new();

        [Description("Frag grenades interaction")]
        public TimedProjectileInteraction FragGrenades { get; set; } = new();

        [Description("Flashbangs interaction")]
        public TimedProjectileInteraction Flashbangs { get; set; } = new();

        [Description("Custom grenades interaction")]
        public ProjectileInteraction CustomGrenades { get; set; } = new();

        [Description("SCP-2176 interaction")]
        public ProjectileInteraction Scp2176 { get; set; } = new();
        [Description("SCP-018 interaction")]
        public TimedProjectileInteraction Scp018 { get; set; } = new();
    }
}
