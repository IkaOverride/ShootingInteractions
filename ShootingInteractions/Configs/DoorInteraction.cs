using System.ComponentModel;

namespace ShootingInteractions.Configs {
    public class DoorInteraction {
        [Description("Is the shooting interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Does the interaction check keycards in the player's inventory")]
        public bool RemoteKeycard { get; set; } = false;

        [Description("Percentage of chance for the buttons to break")]        
        public byte ButtonsBreakChance { get; set; } = 0;

        [Description("Should the door move/do its animation before the buttons break")]
        public bool MoveBeforeBreaking { get; set; } = false;

        [Description("For how long should the buttons stay broken (0 => infinite)")]
        public float ButtonsBreakTime { get; set; } = 0;
    }
}
