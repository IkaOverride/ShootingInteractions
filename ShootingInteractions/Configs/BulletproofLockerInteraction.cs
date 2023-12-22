using System.ComponentModel;

namespace ShootingInteractions.Configs {

    public class BulletproofLockerInteraction {

        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should it check keycards in the player's inventory")]
        public bool RemoteKeycard { get; set; } = true;

        [Description("Should it only work when shooting on the keypad (false = whole interactable)")]
        public bool OnlyKeypad { get; set; } = true;
    }
}
