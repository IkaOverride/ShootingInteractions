using System.ComponentModel;

namespace ShootingInteractions.Configuration.Bases
{
    public class TimedProjectilesInteraction
    {
        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("What's the weapon's minimum armor penetration percentage for the interaction to occur (0 = disabled)")]
        public float MinimumPenetration { get; set; } = 0;

        [Description("Chance to have a custom fuse time (0 = disabled | 100 = always)")]
        public byte CustomFuseTimeChance { get; set; } = 0;

        [Description("Custom fuse time (in seconds)")]
        public float CustomFuseTimeDuration { get; set; } = 0;

        [Description("Should the bullet affect the object's velocity")]
        public bool AdditionalVelocity { get; set; } = false;

        [Description("The force of the bullet that gets multiplied with the object velocity")]
        public float VelocityForce { get; set; } = 7.5f;

        [Description("Should the bullet armor penetration affect the object's velocity")]
        public bool ScaleWithPenetration { get; set; } = false;

        [Description("Constant that gets multiplied with the object velocity and bullet armor penetration")]
        public float VelocityPenetrationMultiplier { get; set; } = 7.5f;
    }
}