using RestoreMonarchy.CustomCommands.Commands;
using RestoreMonarchy.CustomCommands.Models;
using RestoreMonarchy.CustomCommands.Storage;
using Rocket.API.Collections;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System;

namespace RestoreMonarchy.CustomCommands
{
    public class CustomCommandsPlugin : RocketPlugin<CustomCommandsConfiguration>
    {
        public static CustomCommandsPlugin Instance { get; private set; }
        public static CustomCommandsConfiguration Config { get; private set; }

        protected override void Load()
        {
            Instance = this;
            Config = Configuration.Instance;
            Provider.onCommenceShutdown += Provider_onCommenceShutdown;
            foreach (CustomCommandConfig commandConfig in Configuration.Instance.CustomCommands)
            {
                CustomCommand customCommand = new(commandConfig);
                R.Commands.Register(customCommand);

                Logger.Log($"Registered custom command {commandConfig.Name} with {commandConfig.Experience} experience, " +
                    $"{commandConfig.Items.Length} items, {commandConfig.Vehicles.Length} vehicles and " +
                    $"{commandConfig.Messages.Length} messages", ConsoleColor.Yellow);
            }
            new Cooldowns();

            Logger.Log($"{Name} 1.23.1.0 has been loaded!", ConsoleColor.Yellow);
        }

        private void Provider_onCommenceShutdown()
        {
            Cooldowns.Instance.CleanUp();
        }

        protected override void Unload()
        {
            Cooldowns.Instance.CleanUp();
            Logger.Log($"{Name} has been loaded!", ConsoleColor.Yellow);
        }

        public override TranslationList DefaultTranslations => new()
        {
            { "WaitCooldown", "There is currently a cooldown for this command, please wait {0} more seconds." }
        };
    }
}
