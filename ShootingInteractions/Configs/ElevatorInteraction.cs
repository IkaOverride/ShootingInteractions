using System.ComponentModel;

namespace ShootingInteractions.Configs {
    public class ElevatorInteraction {
        [Description("Is the shooting interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Percentage of chance for the buttons to break")]
        public byte ButtonsBreakChance { get; set; } = 0;

        [Description("For how long should the buttons stay broken (0 = infinite)")]
        public float ButtonsBreakTime { get; set; } = 10;

        [Description("Should the elevator still move/do its animation if the buttons should break")]
        public bool MoveBeforeBreaking { get; set; } = false;
    }
}
