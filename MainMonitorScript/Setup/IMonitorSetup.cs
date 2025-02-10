using System.Collections.Generic;
using Sandbox.ModAPI.Ingame;

namespace IngameScript
{
    partial class Program
    {
        public interface IMonitorSetup
        {
            MonitorCreatorConfig setup(IMyGridTerminalSystem grid);
        }
        
        /*
         * Методы для создания дисплеев (отображение)
         */

        private static IDisplay GetDefaultDisplay(IMyGridTerminalSystem grid, MonitorCreatorConfig config, string name, int length)
        {
            if (grid.GetBlockWithName(config.gridPrefix + config.displayPrefix + name) == null) return null;
            
            return new Display(
                textPanel: grid.GetBlockWithName(config.gridPrefix + config.displayPrefix + name) as IMyTextPanel,
                length: length,
                fontSize: 51.0f / length,
                fontColor: config.fontColor,
                backgroundColor: config.backgroundColor
            );
        }

        private static IDisplay GetBigDisplay(IMyGridTerminalSystem grid, MonitorCreatorConfig config, string name, int length)
        {
            if (grid.GetBlockWithName(config.gridPrefix + config.displayPrefix + name) == null) return null;
            
            return new Display(
                textPanel: grid.GetBlockWithName(config.gridPrefix + config.displayPrefix + name) as IMyTextPanel,
                length: length,
                fontSize: 25.5f / length,
                fontColor: config.fontColor,
                backgroundColor: config.backgroundColor
            );
        }

        private static IDisplay GetPairedBigDisplay(IMyGridTerminalSystem grid, MonitorCreatorConfig config, string name, int length)
        {
            if (grid.GetBlockWithName(config.gridPrefix + config.displayPrefix + name + " (1)") == null) return null;
            if (grid.GetBlockWithName(config.gridPrefix + config.displayPrefix + name + " (2)") == null) return null;
            
            int padding = 2;
            return new DisplayGroup(
                textPanels: new List<IMyTextPanel> {
                    grid.GetBlockWithName(config.gridPrefix + config.displayPrefix + name + " (1)") as IMyTextPanel,
                    grid.GetBlockWithName(config.gridPrefix + config.displayPrefix + name + " (2)") as IMyTextPanel
                },
                length: length,
                fontSize: 52.5f / (length + padding*2),
                fontColor: config.fontColor,
                backgroundColor: config.backgroundColor,
                padding: padding
            );
        }
        
        private static IDisplay GetInnerDisplay(IMyGridTerminalSystem grid, MonitorCreatorConfig config, string name, int index, int length)
        {
            if (grid.GetBlockWithName(config.gridPrefix + name) == null) return null;
            if ((grid.GetBlockWithName(config.gridPrefix + name) as IMyTextSurfaceProvider).SurfaceCount <= index) return null;
            
            return new Display(
                textPanel: (grid.GetBlockWithName(config.gridPrefix + name) as IMyTextSurfaceProvider).GetSurface(index),
                length: length,
                fontSize: 25.5f / length,
                fontColor: config.fontColor,
                backgroundColor: config.backgroundColor
            );
        }
    }
}