using System;
using Exiled.API.Features;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace TargetDoor {
    public class TargetDoor : Plugin<Config> {
        private static readonly TargetDoor Singleton = new TargetDoor();

        public override string Name => "TargetDoor";
        public override string Author => "Ika";
        public override Version RequiredExiledVersion => new Version(4, 2, 5);
        public override Version Version => new Version(1, 0, 0);

        private EventsHandler eventsHandler;

        private TargetDoor() { }
        public static TargetDoor Instance => Singleton;

        public override void OnEnabled() {
            RegisterEvents();
            base.OnEnabled();
        }

        public override void OnDisabled() {
            UnregisterEvents();
            base.OnDisabled();
        }

        public void RegisterEvents() {
            eventsHandler = new EventsHandler();

            PlayerEvent.Shooting += eventsHandler.OnShooting;
        }

        public void UnregisterEvents() {
            PlayerEvent.Shooting -= eventsHandler.OnShooting;

            eventsHandler = null;
        }
    }
}
