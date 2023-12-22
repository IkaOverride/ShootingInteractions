using System.ComponentModel;

namespace ShootingInteractions.Configs {

    public class LockerInteraction {

        [Description("Is the interaction enabled")]
        public bool IsEnabled { get; set; } = true;

        [Description("Should it check keycards in the player's inventory")]
        public bool RemoteKeycard { get; set; } = true;
    }
}
