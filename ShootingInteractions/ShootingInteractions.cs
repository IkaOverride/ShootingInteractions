using Exiled.API.Features;
using ShootingInteractions.Configs;
using System;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace ShootingInteractions {
    public class ShootingInteractions : Plugin<Config> {
        private static readonly ShootingInteractions Singleton = new();

        public override string Name => "ShootingInteractions";
        public override string Author => "Ika";
        public override Version RequiredExiledVersion => new(8, 2, 1);
        public override Version Version => new(2, 0, 0);

        private EventsHandler eventsHandler;

        private ShootingInteractions() { }

        public static ShootingInteractions Instance => Singleton;

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
