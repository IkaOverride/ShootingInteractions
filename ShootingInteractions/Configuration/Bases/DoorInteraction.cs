using System.ComponentModel;

namespace ShootingInteractions.Configuration.Bases
{
    public class DoorInteraction
    {
        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should it check keycards in the player's inventory")]
        public bool RemoteKeycard { get; set; } = false;

        [Description("Chance for the door to get locked after shooting (0 = disabled | 100 = always)")]
        public byte LockChance { get; set; } = 0;

        [Description("For how long should the door stay locked (0 or less = infinite)")]
        public float LockDuration { get; set; } = 10;

        [Description("Should the door open/close before getting locked")]
        public bool MoveBeforeLocking { get; set; } = false;

        [Description("What's the minimum ammo penetration to the interaction occour (0 = disabled)")]
        public float MinimumPenetration { get; set; } = 0;
    }
}
