using RestoreMonarchy.CustomCommands.Models;
using RestoreMonarchy.CustomCommands.Storage;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RestoreMonarchy.CustomCommands.Commands
{
    public class CustomCommand : IRocketCommand
    {
        public readonly CustomCommandConfig config;

        public CustomCommand(CustomCommandConfig config)
        {
            this.config = config;
        }

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => config.Name;

        public string Help => config.Help;

        public string Syntax => "";

        public List<string> Aliases => new();

        public List<string> Permissions => new();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (caller is UnturnedPlayer player)
            {
                if (!Cooldowns.Instance.CheckValid(config, player.CSteamID))
                {
                    ChatManager.serverSendMessage(CustomCommandsPlugin.Instance.Translate("WaitCooldown", Cooldowns.Instance.SecondsToWait(config, player.CSteamID)), UnturnedChat.GetColorFromName(CustomCommandsPlugin.Config.DefaultMessageColor, Color.green), null, player.SteamPlayer(), iconURL: CustomCommandsPlugin.Config.DefaultIconUrl, useRichTextFormatting: true);
                    return;
                }

                foreach (ushort item in config.Items)
                    player.GiveItem(item, 1);
                foreach (ushort vehicle in config.Vehicles)
                    player.GiveVehicle(vehicle);
                if (config.Experience > 0)
                    player.Experience += config.Experience;

                foreach (CustomMessage msg in config.Messages)
                {
                    ChatManager.serverSendMessage(msg.Text.Replace('{', '<').Replace('}', '>'), UnturnedChat.GetColorFromName(msg.Color, Color.green), null, player.SteamPlayer(), iconURL: msg.IconUrl, useRichTextFormatting: true);
                }

                Cooldowns.Instance.SetPlayerCooldown(config.Name, player.CSteamID, true);
            }
        }
    }
}
