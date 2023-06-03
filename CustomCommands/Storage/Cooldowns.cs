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
        public static Cooldowns Instance { get; set; }

        public Dictionary<string, Dictionary<CSteamID, DateTime>> PlayerCooldowns { get; private set; }

        public Cooldowns()
        {
            Instance = this;
            PlayerCooldowns = new();
            U.Events.OnPlayerConnected += ProcessPlayerConnect;
            U.Events.OnPlayerDisconnected += ProcessPlayerDisconnect;

            foreach (CustomCommandConfig config in CustomCommandsPlugin.Config.CustomCommands)
            {
                PlayerCooldowns.Add(config.Name, new());
            }

            string dir = Path.Combine(CustomCommandsPlugin.Instance.Directory, "data");

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        private void ProcessPlayerDisconnect(UnturnedPlayer player)
        {
            WritePlayerData(player.CSteamID);
            ClearLoadedData(player.CSteamID);
        }

        private void ProcessPlayerConnect(UnturnedPlayer player)
        {
            ReadPlayerData(player.CSteamID);
        }

        public void WritePlayerData(CSteamID player)
        {
            string dir = Path.Combine(CustomCommandsPlugin.Instance.Directory, $"data/{player}/data.dat");
            Block block = new();
            foreach (var entry in PlayerCooldowns)
            {
                block.writeString(entry.Key);
                block.writeInt64(entry.Value[player].Ticks);
            }
            ReadWrite.writeBlock(dir, false, false, block);
        }

        public void ReadPlayerData(CSteamID player)
        {

            string dir = Path.Combine(CustomCommandsPlugin.Instance.Directory, $"data/{player}/data.dat");
            if (!File.Exists(dir))
            {
                foreach (var entry in PlayerCooldowns)
                {
                    PlayerCooldowns[entry.Key].Add(player, default);
                }
                return;
            }

            Block block = ReadWrite.readBlock(dir, false, false, 0);
            for (byte b = 0; b < PlayerCooldowns.Count; b++)
            {
                string command = block.readString();
                long ticks = block.readInt64();

                if (command != string.Empty)
                    PlayerCooldowns[command].Add(player, new DateTime(ticks));
            }

            foreach (var entry in PlayerCooldowns)
            {
                if (!entry.Value.ContainsKey(player))
                {
                    entry.Value.Add(player, default);
                }
            }
        }

        public void ClearLoadedData(CSteamID player)
        {
            foreach (var entry in PlayerCooldowns)
            {
                entry.Value.Remove(player);
            }
        }

        public bool CheckValid(CustomCommandConfig config, CSteamID player)
        {
            TimeSpan time = DateTime.Now - PlayerCooldowns[config.Name][player];
            if (time.TotalSeconds >= config.Cooldown)
                return true;
            else
                return false;
        }

        public int SecondsToWait(CustomCommandConfig config, CSteamID player)
        {
            TimeSpan time = TimeSpan.FromSeconds(config.Cooldown) - (DateTime.Now - PlayerCooldowns[config.Name][player]);
            return (int)time.TotalSeconds;
        }

        public void SetPlayerCooldown(string command, CSteamID player, bool save = false)
        {
            PlayerCooldowns[command][player] = DateTime.Now;
            if (save)
                WritePlayerData(player);
        }


        public void CleanUp()
        {
            foreach (SteamPlayer client in Provider.clients)
            {
                WritePlayerData(client.playerID.steamID);
                ClearLoadedData(client.playerID.steamID);
            }
            U.Events.OnPlayerConnected -= ProcessPlayerConnect;
            U.Events.OnPlayerDisconnected -= ProcessPlayerDisconnect;
            PlayerCooldowns = null;
        }
    }
}
