using System.ComponentModel;

namespace ShootingInteractions.Configuration.Bases
{
    public class ProjectilesInteraction
    {
        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("What's the weapon's minimum armor penetration percentage for the interaction to occur (0 = disabled)")]
        public float MinimumPenetration { get; set; } = 0;
    }
}
