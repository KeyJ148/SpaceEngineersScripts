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
                
                config.componentsMonitorEnable = true;
                config.componentsDisplay1 = GetDefaultDisplay(grid, config, "компоненты-1", displaySize); 
                config.componentsDisplay2 = GetDefaultDisplay(grid, config, "компоненты-2", displaySize); 
                config.componentsCountByItem = new Dictionary<Item, long>
                {
                    { Items.Components.STEEL_PLATE, 100000 },
                    { Items.Components.INTEROR_PLATE, 100000 },
                    { Items.Components.CONSTRUCTION, 100000 },
                    { Items.Components.COMPUTER, 50000 },
                    { Items.Components.MOTOR, 50000 },
                    { Items.Components.GIRDER, 50000 },
                    { Items.Components.SMALL_TUBE, 50000 },
                    { Items.Components.LARGE_TUBE, 10000 },
                    { Items.Components.METAL_GRID, 10000 },
                    { Items.Components.DISPLAY, 10000 },
                    { Items.Components.BULLETPROOF_GLASS, 10000 },
                    { Items.Components.POWER_CELL, 10000 },
                    { Items.Components.RADIO_COMMUNICATION, 10000 },
                    { Items.Components.MEDICAL, 10000 },
                    { Items.Components.REACTOR, 10000 },
                    { Items.Components.THRUST, 10000 },
                    { Items.Components.DETECTOR, 10000 },
                    { Items.Components.GRAVITY_GENERATOR, 10000 },
                    { Items.Components.EXPLOSIVES, 10000 },
                    { Items.Components.SOLAR_CELL, 10000 },
                    { Items.Components.SUPERCONDUCTOR, 10000 }
                };
                
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
                
                config.groupContainersMonitorEnable = true;
                config.groupContainersDisplay = GetDefaultDisplay(grid, config, "хранилища (группы)", displaySize);
                config.groupContainersNameByDisplayedName = new Dictionary<string, string>
                {
                    {"Железо", "Контейнеры (железо)"},
                    {"Кобальт", "Контейнеры (кобальт)"},
                    {"Общий", "Контейнеры (общие)"},
                };
                
                config.assemblersMonitorEnable = true;
                config.assemblersDisplay = GetDefaultDisplay(grid, config, "сборщики", displaySize);
                
                config.gridContainersMonitorEnable = true;
                config.gridContainersDisplay = GetDefaultDisplay(grid, config, "хранилища", displaySize);
                
                config.batteriesMonitorEnable = true;
                config.batteriesDisplay = GetDefaultDisplay(grid, config, "батареи", displaySize);

                return config;
            }
        }
    }
}