using System.ComponentModel;

namespace ShootingInteractions.Configs
{
    public class ProjectileInteraction
    {
        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;
    }
}
