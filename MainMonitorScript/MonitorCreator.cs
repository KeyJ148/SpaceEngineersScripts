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
            private readonly string gridPrefix;
            
            private readonly string displayPrefix;
            private readonly int displaySize;
            private readonly Color fontColor;
            private readonly Color backgroundColor;
            private readonly ProgressbarSettings progressbarSettings;
            
            private readonly bool oresMonitorEnable;
            private readonly IDisplay oresDisplay; 
            private readonly long oresMaxCount;
            
            private readonly bool ingotsMonitorEnable;
            private readonly IDisplay ingotsDisplay; 
            private readonly long ingotMaxCount;
            
            private readonly bool componentsMonitorEnable;
            private readonly IDisplay componentsDisplay1; 
            private readonly IDisplay componentsDisplay2; 
            private readonly Dictionary<Item, long> componentsCountByItem;
            
            private readonly bool refinesMonitorEnable;
            private readonly IDisplay refinesDisplay;
            private readonly Dictionary<Ore, int> refinesCountByOres;
            private readonly int refinesUniversalCount;
            
            private readonly bool assemblersMonitorEnable;
            private readonly IDisplay assemblersDisplay;
            
            private readonly bool containersMonitorEnable;
            private readonly IDisplay containersDisplay;
            
            private readonly bool hydrogenMonitorEnable;
            private readonly IDisplay hydrogenDisplay;
            private readonly string hydrogenTankTypeKeyword;
            
            private readonly bool oxygenMonitorEnable;
            private readonly IDisplay oxygenDisplay;
            private readonly string oxygenTankTypeKeyword;
            
            private readonly bool batteriesMonitorEnable;
            private readonly IDisplay batteriesDisplay;

            private List<IMyEntity> allContainers = new List<IMyEntity>();
            private IMyGridTerminalSystem grid;
            
            public MonitorCreator(IMyGridTerminalSystem grid)
            {
                this.grid = grid;
                grid.GetBlocksOfType(allContainers);
                
                gridPrefix = "[Vein-11] ";
                
                displayPrefix = "Дисплей - ";
                displaySize = 50;
                fontColor = new Color(140, 140, 140);
                backgroundColor = new Color(2, 2, 2);
                progressbarSettings = new ProgressbarSettings(' ', '■', '■', 0);
                
                oresMonitorEnable = true;
                oresDisplay = GetDefaultDisplay("руды");
                oresMaxCount = 100000L;
                
                ingotsMonitorEnable = true;
                ingotsDisplay = GetDefaultDisplay("слитки"); 
                ingotMaxCount = oresMaxCount * 3;

                componentsMonitorEnable = true;
                componentsDisplay1 = GetDefaultDisplay("компоненты-1"); 
                componentsDisplay2 = GetDefaultDisplay("компоненты-2"); 
                componentsCountByItem = new Dictionary<Item, long>
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
                
                refinesMonitorEnable = true;
                refinesDisplay = GetDefaultDisplay("заводы");
                refinesUniversalCount = 0;
                refinesCountByOres = new Dictionary<Ore, int>
                {
                    { Items.Ores.STONE, 4 },
                    { Items.Ores.IRON, 4 },
                    { Items.Ores.SILICON, 4 },
                    { Items.Ores.NICKEL, 4 },
                    //{ Items.Ores.COBALT, 0 },
                    { Items.Ores.MAGNESIUM, 16 },
                    //{ Items.Ores.SILVER, 0 },
                    //{ Items.Ores.GOLD, 0 },
                    //{ Items.Ores.PLATINUM, 0 },
                    //{ Items.Ores.URANIUM, 0 }
                };
                
                assemblersMonitorEnable = true;
                assemblersDisplay = GetDefaultDisplay("сборщики");
                
                containersMonitorEnable = true;
                containersDisplay = GetDefaultDisplay("хранилища");
                
                hydrogenMonitorEnable = true;
                hydrogenDisplay = GetDefaultDisplay("водород");
                hydrogenTankTypeKeyword = "Водород";
                
                oxygenMonitorEnable = true;
                oxygenDisplay = GetDefaultDisplay("кислород");
                oxygenTankTypeKeyword = "Кислород";
                
                batteriesMonitorEnable = true;
                batteriesDisplay = GetDefaultDisplay("батареи");
            }

            public List<IMonitor> CreateAllMonitors()
            {
                var monitors = new List<IMonitor>();
                if (oresMonitorEnable) monitors.Add(CreateOresMonitor());
                if (ingotsMonitorEnable) monitors.Add(CreateIngotsMonitor());
                if (componentsMonitorEnable) monitors.AddRange(CreateComponentsMonitors());
                if (refinesMonitorEnable) monitors.Add(CreateRefinesMonitor());
                if (assemblersMonitorEnable) monitors.Add(CreateAssemblersMonitor());
                if (containersMonitorEnable) monitors.Add(CreateContainersMonitor());
                if (hydrogenMonitorEnable) monitors.Add(CreateHydrogenMonitor());
                if (oxygenMonitorEnable) monitors.Add(CreateOxygenMonitor());
                if (batteriesMonitorEnable) monitors.Add(CreateBatteriesMonitor());
                
                return monitors;
            }
            
            /*
             * Методы для создания мониторов (наблюдение) 
             */

            private IMonitor CreateOresMonitor()
            {
                return new CargoItemsMonitor(
                    display: oresDisplay,
                    containers: allContainers,
                    itemToMaxCount: Items.ORES.ToDictionary(
                        item => (Item) item,
                        item => oresMaxCount),
                    headerText: "РУДЫ",
                    progressbarSettings: progressbarSettings
                );
            }

            private IMonitor CreateIngotsMonitor()
            {
                return new CargoItemsMonitor(
                    display: ingotsDisplay,
                    containers: allContainers,
                    itemToMaxCount: Items.ORES.ToDictionary(
                        ore => ore.Ingot, 
                        ore => (long)(ore.RefineEfficiency * ingotMaxCount)),
                    headerText: "СЛИТКИ",
                    progressbarSettings: progressbarSettings
                );
            }

            private List<IMonitor> CreateComponentsMonitors()
            {
                IMonitor componentMonitor1 = new CargoItemsMonitor(
                    display: componentsDisplay1,
                    containers: allContainers,
                    itemToMaxCount: Items.COMPONENTS
                        .GetRange(0, Items.COMPONENTS.Count / 2)
                        .ToDictionary(
                            item => (Item) item,
                            item => componentsCountByItem.GetValueOrDefault(item, 0)),
                    headerText: "КОМПОНЕНТЫ",
                    progressbarSettings: progressbarSettings
                );
                
                IMonitor componentMonitor2 = new CargoItemsMonitor(
                    display: componentsDisplay2,
                    containers: allContainers,
                    itemToMaxCount: Items.COMPONENTS
                        .GetRange(Items.COMPONENTS.Count / 2, Items.COMPONENTS.Count - Items.COMPONENTS.Count / 2)
                        .ToDictionary(
                            item => (Item) item,
                            item => componentsCountByItem.GetValueOrDefault(item, 0)),
                    headerText: "КОМПОНЕНТЫ",
                    progressbarSettings: progressbarSettings
                );
                
                return new List<IMonitor> { componentMonitor1, componentMonitor2 };
            }

            private IMonitor CreateRefinesMonitor()
            {
                return new RefinersMonitor(
                    display: refinesDisplay,
                    displayedOres: refinesCountByOres.Keys.ToList(),
                    countRefinersByOreType: refinesCountByOres,
                    countUniversalRefiners: refinesUniversalCount,
                    containers: allContainers,
                    headerText: "ОЧИСТИТЕЛЬНЫЕ ЗАВОДЫ"
                );
            }

            private IMonitor CreateAssemblersMonitor()
            {
                var allAssemblers = new List<IMyAssembler>();
                grid.GetBlocksOfType(allAssemblers);
                
                return new AssemblersMonitor(
                    display: assemblersDisplay,
                    assemblers: allAssemblers,
                    headerText: "СБОРЩИКИ"
                );
            }

            private IMonitor CreateContainersMonitor()
            {
                List<IMyCubeBlock> allContainersBlocks = new List<IMyCubeBlock>();
                grid.GetBlocksOfType(allContainersBlocks);
                var groupContainersByName = Utils.GetBlocksByGridName(allContainersBlocks).
                        ToDictionary(kv => kv.Key, kv => kv.Value.Select(v => (IMyEntity) v).ToList());
                
                return new CargoVolumeMonitor(
                    display: containersDisplay,
                    groupEntityByName: groupContainersByName,
                    headerText: "ХРАНИЛИЩА",
                    progressbarSettings: progressbarSettings
                );
            }

            private IMonitor CreateHydrogenMonitor()
            {
                var gasTanks = new List<IMyGasTank>();
                grid.GetBlocksOfType(gasTanks);
                var hydrogenTanks = gasTanks.FindAll(tank => 
                    tank.DefinitionDisplayNameText.ToLower().Contains(hydrogenTankTypeKeyword.ToLower()));
                var groupHydrogenTanksByName = Utils.GetBlocksByGridName(hydrogenTanks);
                
                return new GasMonitor(
                    display: hydrogenDisplay,
                    groupEntityByName: groupHydrogenTanksByName,
                    headerText: "ВОДОРОД",
                    progressbarSettings: progressbarSettings
                );
            }
            
            private IMonitor CreateOxygenMonitor()
            {
                var gasTanks = new List<IMyGasTank>();
                grid.GetBlocksOfType(gasTanks);
                var oxygenTanks = gasTanks.FindAll(tank => 
                    tank.DefinitionDisplayNameText.ToLower().Contains(oxygenTankTypeKeyword.ToLower()));
                var groupOxygenTanksByName = Utils.GetBlocksByGridName(oxygenTanks);
                
                return new GasMonitor(
                    display: oxygenDisplay,
                    groupEntityByName: groupOxygenTanksByName,
                    headerText: "КИСЛОРОД",
                    progressbarSettings: progressbarSettings
                );
            }

            private IMonitor CreateBatteriesMonitor()
            {
                var batteries = new List<IMyBatteryBlock>();
                grid.GetBlocksOfType(batteries);
                var groupBatteriesByName = Utils.GetBlocksByGridName(batteries);
                
                return new BatteryMonitor(
                    display: batteriesDisplay,
                    groupEntityByName: groupBatteriesByName,
                    headerText: "БАТАРЕИ",
                    progressbarSettings: progressbarSettings
                );
            }
            
            /*
             * Методы для создания дисплеев (отображение)
             */

            private IDisplay GetDefaultDisplay(string name, int length)
            {
                if (grid.GetBlockWithName(gridPrefix + displayPrefix + name) == null) return null;
                
                return new Display(
                    textPanel: grid.GetBlockWithName(gridPrefix + displayPrefix + name) as IMyTextPanel,
                    length: length,
                    fontSize: 51.0f / length,
                    fontColor: fontColor,
                    backgroundColor: backgroundColor
                );
            }

            private IDisplay GetBigDisplay(string name, int length)
            {
                if (grid.GetBlockWithName(gridPrefix + displayPrefix + name) == null) return null;
                
                return new Display(
                    textPanel: grid.GetBlockWithName(gridPrefix + displayPrefix + name) as IMyTextPanel,
                    length: length,
                    fontSize: 25.5f / length,
                    fontColor: fontColor,
                    backgroundColor: backgroundColor
                );
            }

            private IDisplay GetPairedBigDisplay(string name, int length)
            {
                if (grid.GetBlockWithName(gridPrefix + displayPrefix + name + " (1)") == null) return null;
                if (grid.GetBlockWithName(gridPrefix + displayPrefix + name + " (2)") == null) return null;
                
                int padding = 2;
                return new DisplayGroup(
                    textPanels: new List<IMyTextPanel> {
                        grid.GetBlockWithName(gridPrefix + displayPrefix + name + " (1)") as IMyTextPanel,
                        grid.GetBlockWithName(gridPrefix + displayPrefix + name + " (2)") as IMyTextPanel
                    },
                    length: length,
                    fontSize: 52.5f / (length + padding*2),
                    fontColor: fontColor,
                    backgroundColor: backgroundColor,
                    padding: padding
                );
            }

            private IDisplay GetDefaultDisplay(string name)
            {
                return GetDefaultDisplay(name, displaySize);
            }

            private IDisplay GetBigDisplay(string name)
            {
                return GetBigDisplay(name, displaySize);
            }

            private IDisplay GetPairedBigDisplay(string name)
            {
                return GetPairedBigDisplay(name, displaySize);
            }
        }
    }
}
