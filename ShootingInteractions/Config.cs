using Exiled.API.Interfaces;
using System.ComponentModel;

namespace ShootingInteractions {
    public sealed class Config : IConfig {

        // Plugin
        [Description("Indicates whether the plugin is enabled or not")]
        public bool IsEnabled { get; set; } = true;

        [Description("Indicates whether the plugin's debug logs are enabled or not")]
        public bool Debug { get; set; } = false;

        //Doors
        [Description("Should the plugin work with doors")]
        public bool Doors { get; set; } = true;

        [Description("Should the plugin work with keycard doors? (false = never opens, true = opens if the keycard is in their inventory)")]
        public bool DoorCheckKeycard { get; set; } = false;

        [Description("Percentage of chance for a door's buttons to break (0 = never break, 100 = always break)")]
        public byte DoorButtonsBreakChance { get; set; } = 0;

        [Description("Let the door move before breaking the buttons")]
        public bool DoorMoveBeforeBreaking { get; set; } = false;

        [Description("For how long should the door's buttons stay broken (0 = infinite)")]
        public float DoorButtonsBreakTime { get; set; } = 15;

        // Elevators
        [Description("Should the plugin work with elevators")]
        public bool Elevators { get; set; } = true;

        [Description("Percentage of chance for an elevator's buttons to break (0 = never break, 100 = always break)")]
        public byte ElevatorButtonsBreakChance { get; set; } = 0;
        
        [Description("Let the elevator move before breaking the buttons")]
        public bool ElevatorMoveBeforeBreaking { get; set; } = false;

        [Description("For how long should the elevator's buttons stay broken (0 = infinite)")]
        public float ElevatorButtonsBreakTime { get; set; } = 15;

        [Description("Should the plugin work with grenades")]
        public bool Grenades { get; set; } = false;
    }
}
