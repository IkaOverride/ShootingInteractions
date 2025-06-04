using System.ComponentModel;

namespace ShootingInteractions.Configuration.Bases
{
    public class LockersInteraction
    {
        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("What's the weapon's minimum armor penetration percentage for the interaction to occur (0 = disabled)")]
        public float MinimumPenetration { get; set; } = 0;

        [Description("Should it check keycards in the player's inventory")]
        public bool RemoteKeycard { get; set; } = true;
    }
}
