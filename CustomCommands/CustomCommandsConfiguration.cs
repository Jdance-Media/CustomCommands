using RestoreMonarchy.CustomCommands.Models;
using Rocket.API;

namespace RestoreMonarchy.CustomCommands
{
    public class CustomCommandsConfiguration : IRocketPluginConfiguration
    {
        public CustomCommandConfig[] CustomCommands { get; set; }

        public void LoadDefaults()
        {
            CustomCommands = new CustomCommandConfig[]
            {
                new CustomCommandConfig()
                {
                    Name = "generator",
                    Help = "Kit with one generator and two portable gas cans.",
                    Cooldown = 10,
                    Experience = 0,
                    Items = new ushort[]
                    {
                        458,
                        28,
                        28
                    },
                    Vehicles = new ushort[0],
                    Messages = new CustomMessage[]
                    {
                        new CustomMessage()
                        {
                            Text = "{b}You received x1 Generator and x2 Portable Gas Cans{/b}.",
                            Color = "red",
                            IconUrl = "https://unturnedstore.com/api/images/3373"
                        }
                    }                    
                }
            };
        }
    }
}