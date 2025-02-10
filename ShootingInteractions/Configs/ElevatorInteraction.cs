using System.ComponentModel;

namespace ShootingInteractions.Configs
{
    public class ElevatorInteraction
    {
        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Chance for the elevator to get locked after shooting (0 = disabled | 100 = always)")]
        public byte ButtonsBreakChance { get; set; } = 0;

        [Description("How long should the elevator stay locked (0 or less = infinite)")]
        public float ButtonsBreakTime { get; set; } = 10;

        [Description("Should the elevator move before getting locked")]
        public bool MoveBeforeBreaking { get; set; } = false;
    }
}
