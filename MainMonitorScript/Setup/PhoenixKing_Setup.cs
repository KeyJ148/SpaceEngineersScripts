using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class PhoenixKing_Setup : IMonitorSetup
        {
            public MonitorCreatorConfig setup(IMyGridTerminalSystem grid)
            {
                MonitorCreatorConfig config = new MonitorCreatorConfig();
                int displaySize = 50;
                
                config.gridPrefix = "[PhoenixKing] ";
                
                config.displayPrefix = "Дисплей - ";
                config.fontColor = new Color(140, 140, 140);
                config.backgroundColor = new Color(2, 2, 2);
                config.progressbarSettings = new ProgressbarSettings(' ', '■', '■', 0);
                
                config.gridContainersMonitorEnable = true;
                config.gridContainersDisplay = GetInnerDisplay(grid, config, "Кокпит", 0, displaySize);
                
                config.hydrogenMonitorEnable = true;
                config.hydrogenDisplay = GetInnerDisplay(grid, config, "Кокпит", 1, displaySize);
                config.hydrogenTankTypeKeyword = "Водород";
                
                config.oxygenMonitorEnable = true;
                config.oxygenDisplay = GetInnerDisplay(grid, config, "Кокпит", 3, displaySize);
                config.oxygenTankTypeKeyword = "Кислород";
                
                config.batteriesMonitorEnable = true;
                config.batteriesDisplay = GetInnerDisplay(grid, config, "Кокпит", 2, displaySize);

                return config;
            }
        }
    }
}