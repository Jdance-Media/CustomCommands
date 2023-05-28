using RestoreMonarchy.CustomCommands.Models;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace RestoreMonarchy.CustomCommands.Commands
{
    public class CustomCommand : IRocketCommand
    {
        private readonly CustomCommandConfig config;

        public CustomCommand(CustomCommandConfig config)
        {
            this.config = config;
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => config.Name;

        public string Help => config.Help;

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (caller is UnturnedPlayer)
            {
                UnturnedPlayer player = (UnturnedPlayer)caller;

                foreach (ushort item in config.Items)
                {
                    player.GiveItem(item, 1);
                }

                foreach (ushort vehicle in config.Vehicles)
                {
                    player.GiveVehicle(vehicle);
                }

                if (config.Experience > 0)
                {
                    player.Experience += config.Experience;
                }
            }                     

            foreach (CustomMessage msg in config.Messages)
            {
                UnturnedChat.Say(caller, msg.Text, UnturnedChat.GetColorFromName(msg.Color, Color.green));
            }
        }
    }
}
