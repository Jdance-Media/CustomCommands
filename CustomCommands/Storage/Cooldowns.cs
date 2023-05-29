using RestoreMonarchy.CustomCommands.Commands;
using RestoreMonarchy.CustomCommands.Models;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestoreMonarchy.CustomCommands.Storage
{
    public class Cooldowns
    {
        public static Cooldowns Instance { get; private set; }

        public Dictionary<string, Dictionary<CSteamID, DateTime>> PlayerCooldowns { get; private set; }

        public Cooldowns()
        {
            Instance = this;
            PlayerCooldowns = new();
            U.Events.OnPlayerConnected += ProcessPlayerConnect;
            U.Events.OnPlayerConnected += ProcessPlayerDisconnect;

            foreach (CustomCommandConfig config in CustomCommandsPlugin.Config.CustomCommands)
            {
                PlayerCooldowns.Add(config.Name, new());
            }

        }

        private void ProcessPlayerDisconnect(UnturnedPlayer player)
        {
            WritePlayerData(player.CSteamID);
        }

        private void ProcessPlayerConnect(UnturnedPlayer player)
        {
            ReadPlayerData(player.CSteamID);
        }

        public void ReadPlayerData(CSteamID player)
        {
            string path = Path.Combine(CustomCommandsPlugin.Instance.Directory, $"data/{player}/cooldowns.dat");
            DatFast dat = new(path);

            if (!dat.Safe) return;

            foreach (CustomCommandConfig config in CustomCommandsPlugin.Config.CustomCommands)
            {
                var data = dat.ReadNonOrder<DateTime>(config.Name);
                if (data == default) continue;

                PlayerCooldowns[config.Name].Add(player, data);
            }
        }

        public void WritePlayerData(CSteamID player)
        {
            string path = Path.Combine(CustomCommandsPlugin.Instance.Directory, $"data/{player}/cooldowns.dat");
            DatFast dat = new(path);

            foreach (CustomCommandConfig config in CustomCommandsPlugin.Config.CustomCommands)
            {
                if (!PlayerCooldowns[config.Name].ContainsKey(player)) continue;
                dat.Write(PlayerCooldowns[config.Name][player]);
                dat.Save();
            }
        }

        public bool ValidCooldown(CustomCommand command, CSteamID player)
        {
            DateTime current = GetPlayerCooldown(command.Name, player);
            if ((DateTime.Now - current).TotalSeconds > command.config.Cooldown)
                return true;
            else
                return false;
        }

        public DateTime GetPlayerCooldown(string command, CSteamID player)
        {
            if (PlayerCooldowns[command].ContainsKey(player))
                return PlayerCooldowns[command][player];
            else
                return default;
        }

        public void SetPlayerCooldown(string command, CSteamID player, bool save = false)
        {
            if (PlayerCooldowns[command].ContainsKey(player)) PlayerCooldowns[command][player] = DateTime.Now;
            else PlayerCooldowns[command].Add(player, DateTime.Now);

            if (save)
                Task.Run(() => WritePlayerData(player));
        }

        public void CleanUp()
        {
            foreach (SteamPlayer client in Provider.clients)
            {
                WritePlayerData(client.playerID.steamID);
            }
            U.Events.OnPlayerConnected -= ProcessPlayerConnect;
            U.Events.OnPlayerConnected -= ProcessPlayerDisconnect;
            PlayerCooldowns = null;
            Instance = null;
        }
    }
}
