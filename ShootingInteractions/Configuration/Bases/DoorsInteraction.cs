using System.ComponentModel;

namespace ShootingInteractions.Configuration.Bases
{
    public class DoorsInteraction
    {
        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("What's the weapon's minimum armor penetration percentage for the interaction to occur (0 = disabled)")]
        public float MinimumPenetration { get; set; } = 0;

        [Description("Should it check keycards in the player's inventory")]
        public bool RemoteKeycard { get; set; } = false;

        [Description("Chance for the door to get locked after shooting (0 = disabled | 100 = always)")]
        public byte LockChance { get; set; } = 0;

        [Description("For how long should the door stay locked (0 or less = infinite)")]
        public float LockDuration { get; set; } = 10;

        [Description("Should the door open/close before getting locked")]
        public bool MoveBeforeLocking { get; set; } = false;
    }
}
