using System.ComponentModel;

namespace ShootingInteractions.Configs
{
    public class TimedProjectileInteraction
    {
        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Chance for the grenade to have a custom fuse time (0 = disabled | 100 = always)")]
        public byte MalfunctionChance { get; set; } = 0;

        [Description("Grenade's custom fuse time (in seconds)")]
        public float MalfunctionFuseTime { get; set; } = 0;

        [Description("Should the grenade get additional velocity to where the player is facing")]
        public bool AdditionalVelocity { get; set; } = false;

        [Description("The force of the velocity that gets added")]
        public float VelocityForce { get; set; } = 7.5f;
    }
}
