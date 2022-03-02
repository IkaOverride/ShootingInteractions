using Exiled.API.Interfaces;
using System.ComponentModel;

namespace TargetDoor {
    public sealed class Config : IConfig {
        [Description("Indicates wether the plugin is enabled or not")]
        public bool IsEnabled { get; set; } = true;
    }
}
