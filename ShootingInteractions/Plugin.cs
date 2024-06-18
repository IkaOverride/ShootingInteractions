using Exiled.API.Features;
using ShootingInteractions.Configs;
using System;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace ShootingInteractions {
    public class Plugin : Plugin<Config> {
        private static readonly Plugin Singleton = new();

        public override string Name => "ShootingInteractions";

        public override string Author => "Ika";

        public override Version RequiredExiledVersion => new(8, 9, 4);

        public override Version Version => new(2, 3, 4);

        private EventsHandler eventsHandler;

        private Plugin() { }

        public static Plugin Instance => Singleton;

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
            PlayerEvent.Shot += eventsHandler.OnShot;
        }

        public void UnregisterEvents() {
            PlayerEvent.Shooting -= eventsHandler.OnShooting;
            PlayerEvent.Shot -= eventsHandler.OnShot;

            eventsHandler = null;
        }
    }
}
