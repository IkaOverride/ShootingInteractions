using System.ComponentModel;

namespace ShootingInteractions.Configs {
    public class BulletproofLockerInteraction {
        [Description("Is the shooting interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Does the interaction check keycards in the player's inventory")]
        public bool RemoteKeycard { get; set; } = true;

        [Description("Should the interaction work only when shooting on the locker's keypad")]
        public bool OnlyKeypad { get; set; } = true;
    }
}
