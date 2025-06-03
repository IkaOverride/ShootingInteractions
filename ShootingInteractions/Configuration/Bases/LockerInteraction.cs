using System.ComponentModel;

namespace ShootingInteractions.Configuration.Bases
{
    public class LockerInteraction
    {
        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should it check keycards in the player's inventory")]
        public bool RemoteKeycard { get; set; } = true;

        [Description("What's the minimum ammo penetration to the interaction occour (0 = disabled)")]
        public float MinimumPenetration { get; set; } = 0;
    }
}
