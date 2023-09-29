using System.ComponentModel;

namespace ShootingInteractions.Configs {
    public class ElevatorInteraction {
        [Description("Is the shooting interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Percentage of chance for the buttons to break")]
        public byte ButtonsBreakChance { get; set; } = 0;

        [Description("Should the door move/do its animation before the buttons break")]
        public bool MoveBeforeBreaking { get; set; } = false;

        [Description("For how long should the buttons stay broken (0 = infinite)")]
        public float ButtonsBreakTime { get; set; } = 0;
    }
}
