using System.ComponentModel;

namespace ShootingInteractions.Configuration.Bases
{
    public class ProjectileInteraction
    {
        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;
    }
}
