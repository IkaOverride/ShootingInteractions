using Exiled.API.Interfaces;
using System.ComponentModel;

namespace TargetDoor {
    public sealed class Config : IConfig {
        [Description("Indicates whether the plugin is enabled or not")]
        public bool IsEnabled { get; set; } = true;

        [Description("Indicates whether the plugin's debug logs are enabled or not")]
        public bool Debug { get; set; } = false;

        [Description("Should a door that requires a keycard open if the player has a keycard to open it")]
        public bool CheckKeycard { get; set; } = false;

        [Description("The percentage of chance for a door's buttons to break when a player shoot on one")]
        public byte DoorBreakChance { get; set; } = 0;

        [Description("Should the button break BEFORE the doors open or close")]
        public bool DoorBreakBefore { get; set; } = true;

        [Description("For how long should the door stay broke (0 = infinite)")]
        public float DoorBreakTime { get; set; } = 15;
    }
}
