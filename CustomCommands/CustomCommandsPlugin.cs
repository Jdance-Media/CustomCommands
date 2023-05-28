using RestoreMonarchy.CustomCommands.Commands;
using RestoreMonarchy.CustomCommands.Models;
using Rocket.Core;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using System;

namespace RestoreMonarchy.CustomCommands
{
    public class CustomCommandsPlugin : RocketPlugin<CustomCommandsConfiguration>
    {
        protected override void Load()
        {
            foreach (CustomCommandConfig commandConfig in Configuration.Instance.CustomCommands)
            {
                CustomCommand customCommand = new CustomCommand(commandConfig);
                R.Commands.Register(customCommand);

                Logger.Log($"Registered custom command {commandConfig.Name} with {commandConfig.Experience} experience, " +
                    $"{commandConfig.Items.Length} items, {commandConfig.Vehicles.Length} vehicles and " +
                    $"{commandConfig.Messages.Length} messages", ConsoleColor.Yellow);
            }

            Logger.Log($"{Name} {Assembly.GetName().Version} has been loaded!", ConsoleColor.Yellow);
        }

        protected override void Unload()
        {
            Logger.Log($"{Name} has been loaded!", ConsoleColor.Yellow);
        }
    }
}
