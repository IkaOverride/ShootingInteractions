using System.ComponentModel;

namespace ShootingInteractions.Configs
{
    public class DoorInteraction
    {
        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should it check keycards in the player's inventory")]
        public bool RemoteKeycard { get; set; } = false;

        [Description("Chance for the door to get locked after shooting (0 = disabled | 100 = always)")]
        public byte ButtonsBreakChance { get; set; } = 0;

        [Description("For how long should the door stay locked (0 or less = infinite)")]
        public float ButtonsBreakTime { get; set; } = 10;

        [Description("Should the door open/close before getting locked")]
        public bool MoveBeforeBreaking { get; set; } = false;
    }
}
