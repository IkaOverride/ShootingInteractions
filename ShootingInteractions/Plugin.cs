using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Pickups;
using ShootingInteractions.Configuration;
using System;
using System.Linq;
using System.Reflection;
using PlayerEvent = Exiled.Events.Handlers.Player;

namespace ShootingInteractions
{
    public class Plugin : Plugin<Config>
    {
        internal static MethodInfo GetCustomItem = null;

        public static Plugin Instance { get; private set; }

        public override string Name => "ShootingInteractions";

        public override string Author => "Ika, Maintained by Unbistrackted";

        public override Version RequiredExiledVersion => new(9, 6, 0);

        public override Version Version => new(2, 5, 0);

        public override PluginPriority Priority => PluginPriority.First;

        private EventsHandler eventsHandler;

        public override void OnEnabled()
        {
            Instance = this;

            Assembly customItems = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == "Exiled.CustomItems");

            if (customItems is not null)
            {
                Type customItemType = customItems.GetType("Exiled.CustomItems.API.Features.CustomItem");
                GetCustomItem = customItemType?.GetMethod("TryGet", new[] { typeof(Pickup), customItemType.MakeByRefType() });
            }

            RegisterEvents();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            UnregisterEvents();

            Instance = null;

            base.OnDisabled();
        }

        public void RegisterEvents()
        {
            eventsHandler = new EventsHandler();

            PlayerEvent.Shot += eventsHandler.OnShot;
        }

        public void UnregisterEvents()
        {
            PlayerEvent.Shot -= eventsHandler.OnShot;

            eventsHandler = null;
        }
    }
}
