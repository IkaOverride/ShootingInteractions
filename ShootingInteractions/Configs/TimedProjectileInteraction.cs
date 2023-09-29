using System.ComponentModel;

namespace ShootingInteractions.Configs {
    public class TimedProjectileInteraction {
        [Description("Is the shooting interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should the projectile explode instantly")]
        public bool ExplodeInstantly { get; set; } = false;
    }
}
