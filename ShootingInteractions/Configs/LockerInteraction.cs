using System.ComponentModel;

namespace ShootingInteractions.Configs {
    public class LockerInteraction {
        [Description("Is the shooting interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Does the interaction check keycards in the player's inventory")]
        public bool RemoteKeycard { get; set; } = true;
    }
}
