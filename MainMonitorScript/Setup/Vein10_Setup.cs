using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class Vein10_Setup : IMonitorSetup
        {
            public MonitorCreatorConfig setup(IMyGridTerminalSystem grid)
            {
                MonitorCreatorConfig config = new MonitorCreatorConfig();
                int displaySize = 50;
                
                config.gridPrefix = "[Vein-10] ";
                
                config.displayPrefix = "Дисплей - ";
                config.fontColor = new Color(140, 140, 140);
                config.backgroundColor = new Color(2, 2, 2);
                config.progressbarSettings = new ProgressbarSettings(' ', '■', '■', 0);
                
                config.oresMonitorEnable = true;
                config.oresDisplay = GetDefaultDisplay(grid, config, "руды", displaySize);
                config.oresMaxCount = 15000000L;
                
                config.ingotsMonitorEnable = true;
                config.ingotsDisplay = GetDefaultDisplay(grid, config, "слитки", displaySize); 
                config.ingotMaxCount = config.oresMaxCount;
                
                config.refinesMonitorEnable = true;
                config.refinesDisplay = GetDefaultDisplay(grid, config, "заводы", displaySize);
                config.refinesUniversalCount = 0;
                config.refinesCountByOres = new Dictionary<Ore, int>
                {
                    { Items.Ores.STONE, 10 },
                    { Items.Ores.IRON, 10 },
                    //{ Items.Ores.SILICON, 4 },
                    //{ Items.Ores.NICKEL, 4 },
                    { Items.Ores.COBALT, 105 },
                    //{ Items.Ores.MAGNESIUM, 16 },
                    //{ Items.Ores.SILVER, 0 },
                    //{ Items.Ores.GOLD, 0 },
                    //{ Items.Ores.PLATINUM, 0 },
                    //{ Items.Ores.URANIUM, 0 }
                };

                return config;
            }
        }
    }
}