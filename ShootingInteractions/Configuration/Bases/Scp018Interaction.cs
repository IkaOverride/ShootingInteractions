using System.ComponentModel;

namespace ShootingInteractions.Configuration.Bases
{
    public class Scp018Interaction
    {
        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Chance for SCP-018 to have a custom fuse time (0 = disabled | 100 = always)")]
        public byte CustomFuseTimeChance { get; set; } = 0;

        [Description("SCP-018's custom fuse time (in seconds)")]
        public float CustomFuseTimeDuration { get; set; } = 0;

        [Description("Should the bullet affect SCP-018's velocity")]
        public bool AdditionalVelocity { get; set; } = true;

        [Description("The force of the bullet that gets multiplied with the object velocity")]
        public float BulletForce { get; set; } = 30f;

        [Description("What's the minimum ammo penetration to the interaction occour (0 = disabled)")]
        public float MinimumPenetration { get; set; } = 0;
    }
}
