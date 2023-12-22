using System.ComponentModel;

namespace ShootingInteractions.Configs {

    public class ElevatorInteraction {

        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Percentage of chance for the elevator to break (0 is disabled)")]
        public byte ButtonsBreakChance { get; set; } = 0;

        [Description("For how long should the elevator stay broken (0 or less is infinite)")]
        public float ButtonsBreakTime { get; set; } = 10;

        [Description("Should the elevator still move/do its animation before breaking")]
        public bool MoveBeforeBreaking { get; set; } = false;
    }
}
