using System.ComponentModel;

namespace ShootingInteractions.Configs {

    public class DoorInteraction {

        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should it check keycards in the player's inventory")]
        public bool RemoteKeycard { get; set; } = false;

        [Description("Percentage of chance for the buttons to break (0 is disabled)")]
        public byte ButtonsBreakChance { get; set; } = 0;

        [Description("For how long should the buttons stay broken (0 or less is infinite)")]
        public float ButtonsBreakTime { get; set; } = 10;

        [Description("Should the door still move/do its animation before breaking")]
        public bool MoveBeforeBreaking { get; set; } = false;
    }
}
