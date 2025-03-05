using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class WaterTribe_Setup : IMonitorSetup
        {
            public MonitorCreatorConfig setup(IMyGridTerminalSystem grid)
            {
                MonitorCreatorConfig config = new MonitorCreatorConfig();
                int displaySize = 50;
                
                config.gridPrefix = "[WaterTribe] ";
                
                config.displayPrefix = "Дисплей - ";
                config.fontColor = new Color(140, 140, 140);
                config.backgroundColor = new Color(2, 2, 2);
                config.progressbarSettings = new ProgressbarSettings(' ', '■', '■', 0);
                
                config.gridContainersMonitorEnable = true;
                config.gridContainersDisplay = GetDefaultDisplay(grid, config, "хранилища", displaySize);
                
                config.hydrogenMonitorEnable = true;
                config.hydrogenDisplay = GetDefaultDisplay(grid, config, "водород", displaySize);
                config.hydrogenTankTypeKeyword = "Водород";
                
                config.oxygenMonitorEnable = true;
                config.oxygenDisplay = GetDefaultDisplay(grid, config, "кислород", displaySize);
                config.oxygenTankTypeKeyword = "Кислород";
                
                config.batteriesMonitorEnable = true;
                config.batteriesDisplay = GetDefaultDisplay(grid, config, "батареи", displaySize);

                return config;
            }
        }
    }
}