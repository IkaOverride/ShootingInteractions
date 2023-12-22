using System.ComponentModel;

namespace ShootingInteractions.Configs {

    public class TimedProjectileInteraction {

        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Percentage of chance for the grenade to malfunction (0 is disabled)")]
        public byte MalfunctionChance { get; set; } = 0;

        [Description("Grenade's fuse time when malfunction occurs")]
        public float MalfunctionFuseTime { get; set; } = 0;

        [Description("Should the grenade get additional velocity to where the player is facing")]
        public bool AdditionalVelocity { get; set; } = false;

        [Description("The force of the velocity that gets added")]
        public float VelocityForce { get; set; } = 7.5f;
    }
}
