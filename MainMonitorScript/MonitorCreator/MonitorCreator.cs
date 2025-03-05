using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class MonitorCreator
        {
            private IMyGridTerminalSystem grid;
            private MonitorCreatorConfig config;
            
            private List<IMyEntity> allBlocks = new List<IMyEntity>();
            private List<IMyCubeBlock> allContainersBlocks = new List<IMyCubeBlock>();
            
            public MonitorCreator(IMyGridTerminalSystem grid, MonitorCreatorConfig config)
            {
                this.grid = grid;
                this.config = config;
            }

            public List<IMonitor> CreateAllMonitors()
            {
                grid.GetBlocksOfType(allBlocks);
                grid.GetBlocksOfType(allContainersBlocks);
                
                var monitors = new List<IMonitor>();
                if (config.oresMonitorEnable) monitors.Add(CreateOresMonitor());
                if (config.ingotsMonitorEnable) monitors.Add(CreateIngotsMonitor());
                if (config.componentsMonitorEnable) monitors.AddRange(CreateComponentsMonitors());
                if (config.refinesMonitorEnable) monitors.Add(CreateRefinesMonitor());
                if (config.assemblersMonitorEnable) monitors.Add(CreateAssemblersMonitor());
                if (config.gridContainersMonitorEnable) monitors.Add(CreateGridContainersMonitor());
                if (config.groupContainersMonitorEnable) monitors.Add(CreateGroupContainersMonitor());
                if (config.hydrogenMonitorEnable) monitors.Add(CreateHydrogenMonitor());
                if (config.oxygenMonitorEnable) monitors.Add(CreateOxygenMonitor());
                if (config.batteriesMonitorEnable) monitors.Add(CreateBatteriesMonitor());
                
                return monitors;
            }
            
            /*
             * Методы для создания мониторов (наблюдение) 
             */

            private IMonitor CreateOresMonitor()
            {
                return new CargoItemsMonitor(
                    display: config.oresDisplay,
                    containers: allBlocks,
                    itemToMaxCount: Items.ORES.ToDictionary(
                        item => (Item) item,
                        item => config.oresMaxCount),
                    headerText: "РУДЫ",
                    progressbarSettings: config.progressbarSettings
                );
            }

            private IMonitor CreateIngotsMonitor()
            {
                return new CargoItemsMonitor(
                    display: config.ingotsDisplay,
                    containers: allBlocks,
                    itemToMaxCount: Items.ORES.ToDictionary(
                        ore => ore.Ingot, 
                        ore => (long)(ore.RefineEfficiency * config.ingotMaxCount)),
                    headerText: "СЛИТКИ",
                    progressbarSettings: config.progressbarSettings
                );
            }

            private List<IMonitor> CreateComponentsMonitors()
            {
                IMonitor componentMonitor1 = new CargoItemsMonitor(
                    display: config.componentsDisplay1,
                    containers: allBlocks,
                    itemToMaxCount: Items.COMPONENTS
                        .GetRange(0, Items.COMPONENTS.Count / 2)
                        .ToDictionary(
                            item => (Item) item,
                            item => config.componentsCountByItem.GetValueOrDefault(item, 0)),
                    headerText: "КОМПОНЕНТЫ",
                    progressbarSettings: config.progressbarSettings
                );
                
                IMonitor componentMonitor2 = new CargoItemsMonitor(
                    display: config.componentsDisplay2,
                    containers: allBlocks,
                    itemToMaxCount: Items.COMPONENTS
                        .GetRange(Items.COMPONENTS.Count / 2, Items.COMPONENTS.Count - Items.COMPONENTS.Count / 2)
                        .ToDictionary(
                            item => (Item) item,
                            item => config.componentsCountByItem.GetValueOrDefault(item, 0)),
                    headerText: "КОМПОНЕНТЫ",
                    progressbarSettings: config.progressbarSettings
                );
                
                return new List<IMonitor> { componentMonitor1, componentMonitor2 };
            }

            private IMonitor CreateRefinesMonitor()
            {
                return new RefinersMonitor(
                    display: config.refinesDisplay,
                    displayedOres: config.refinesCountByOres.Keys.ToList(),
                    countRefinersByOreType: config.refinesCountByOres,
                    countUniversalRefiners: config.refinesUniversalCount,
                    containers: allBlocks,
                    headerText: "ОЧИСТИТЕЛЬНЫЕ ЗАВОДЫ"
                );
            }

            private IMonitor CreateAssemblersMonitor()
            {
                var allAssemblers = new List<IMyAssembler>();
                grid.GetBlocksOfType(allAssemblers);
                
                return new AssemblersMonitor(
                    display: config.assemblersDisplay,
                    assemblers: allAssemblers,
                    headerText: "СБОРЩИКИ"
                );
            }

            private IMonitor CreateGridContainersMonitor()
            {
                var groupContainersByName = Utils.GetBlocksByGridName(allContainersBlocks).
                        ToDictionary(kv => kv.Key, kv => kv.Value.Select(v => (IMyEntity) v).ToList());
                
                return new CargoVolumeMonitor(
                    display: config.gridContainersDisplay,
                    groupEntityByName: groupContainersByName,
                    headerText: "ХРАНИЛИЩА",
                    progressbarSettings: config.progressbarSettings
                );
            }
            
            private IMonitor CreateGroupContainersMonitor()
            {
                var groupContainersByName = config.groupContainersNameByDisplayedName.
                    ToDictionary(kv => kv.Key, kv => { 
                        List<IMyTerminalBlock> groupOfContainers = new List<IMyTerminalBlock>();
                        grid.GetBlockGroupWithName(config.gridPrefix + kv.Value).GetBlocks(groupOfContainers);
                        return groupOfContainers.Select(v => (IMyEntity) v).ToList(); 
                    });
                
                return new CargoVolumeMonitor(
                    display: config.groupContainersDisplay,
                    groupEntityByName: groupContainersByName,
                    headerText: "ХРАНИЛИЩА",
                    progressbarSettings: config.progressbarSettings
                );
            }

            private IMonitor CreateHydrogenMonitor()
            {
                var gasTanks = new List<IMyGasTank>();
                grid.GetBlocksOfType(gasTanks);
                var hydrogenTanks = gasTanks.FindAll(tank => 
                    tank.DefinitionDisplayNameText.ToLower().Contains(config.hydrogenTankTypeKeyword.ToLower()));
                var groupHydrogenTanksByName = Utils.GetBlocksByGridName(hydrogenTanks);
                
                return new GasMonitor(
                    display: config.hydrogenDisplay,
                    groupEntityByName: groupHydrogenTanksByName,
                    headerText: "ВОДОРОД",
                    progressbarSettings: config.progressbarSettings
                );
            }
            
            private IMonitor CreateOxygenMonitor()
            {
                var gasTanks = new List<IMyGasTank>();
                grid.GetBlocksOfType(gasTanks);
                var oxygenTanks = gasTanks.FindAll(tank => 
                    tank.DefinitionDisplayNameText.ToLower().Contains(config.oxygenTankTypeKeyword.ToLower()));
                var groupOxygenTanksByName = Utils.GetBlocksByGridName(oxygenTanks);
                
                return new GasMonitor(
                    display: config.oxygenDisplay,
                    groupEntityByName: groupOxygenTanksByName,
                    headerText: "КИСЛОРОД",
                    progressbarSettings: config.progressbarSettings
                );
            }

            private IMonitor CreateBatteriesMonitor()
            {
                var batteries = new List<IMyBatteryBlock>();
                grid.GetBlocksOfType(batteries);
                var groupBatteriesByName = Utils.GetBlocksByGridName(batteries);
                
                return new BatteryMonitor(
                    display: config.batteriesDisplay,
                    groupEntityByName: groupBatteriesByName,
                    headerText: "БАТАРЕИ",
                    progressbarSettings: config.progressbarSettings
                );
            }
        }
    }
}
