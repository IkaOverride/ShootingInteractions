using Exiled.API.Interfaces;
using System.ComponentModel;

namespace ShootingInteractions.Configs {
    public sealed class Config : IConfig {

        [Description("Indicates whether the plugin is enabled or not")]
        public bool IsEnabled { get; set; } = true;

        [Description("Indicates whether the plugin's debug logs are enabled or not")]
        public bool Debug { get; set; } = false;

        [Description("Normal doors buttons")]
        public DoorInteraction Doors { get; set; } = new();

        [Description("Checkpoint doors buttons")]
        public DoorInteraction Checkpoints { get; set; } = new();

        [Description("Gates buttons")]
        public DoorInteraction Gates { get; set; } = new();

        [Description("Bulletproof lockers keycard readers")]
        public LockerInteraction BulletproofLockers { get; set; } = new();

        [Description("Elevators buttons")]
        public ElevatorInteraction Elevators { get; set; } = new();

        [Description("Frag grenades")]
        public TimedProjectileInteraction FragGrenades { get; set; } = new();

        [Description("Flashbangs")]
        public TimedProjectileInteraction Flashbangs { get; set; } = new();

        [Description("SCP-2176")]
        public ProjectileInteraction Scp2176 { get; set; } = new();
    }
}
