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
            private const string GRID_PREFIX = "[GTW] ";
            private const string DISPLAY_PREFIX = "Дисплей - ";
            private const int DISPLAY_SIZE = 50;
            private readonly Color FONT_COLOR = new Color(2, 63, 2);
            private readonly Color BACKGROUND_COLOR = new Color(2, 2, 2);
            private readonly ProgressbarSettings PROGRESSBAR_SETTINGS = new ProgressbarSettings(' ', '■', '■', 0);

            private const long ORES_MAX_COUNT = 100000L;
            private const long INGOT_MAX_COUNT = ORES_MAX_COUNT * 3;

            private IMyGridTerminalSystem grid;

            public MonitorCreator(IMyGridTerminalSystem grid)
            {
                this.grid = grid;
            }

            public List<IMonitor> CreateAllMonitors()
            {
                var result = new List<IMonitor>();

                var allContainers = new List<IMyEntity>();
                grid.GetBlocksOfType(allContainers);
                var allAssemblers = new List<IMyAssembler>();
                grid.GetBlocksOfType(allAssemblers);

                result.Add(new CargoItemsMonitor(
                    display: GetPairedBigDisplay("руды"),
                    containers: allContainers,
                    itemToMaxCount: Items.ORES.ToDictionary(item => (Item)item, item => ORES_MAX_COUNT),
                    headerText: "РУДЫ",
                    progressbarSettings: PROGRESSBAR_SETTINGS
                ));

                result.Add(new CargoItemsMonitor(
                    display: GetPairedBigDisplay("слитки"),
                    containers: allContainers,
                    itemToMaxCount: Items.ORES.ToDictionary(ore => ore.Ingot, ore => (long)(ore.RefineEfficiency * INGOT_MAX_COUNT)),
                    headerText: "СЛИТКИ",
                    progressbarSettings: PROGRESSBAR_SETTINGS
                 ));

                result.Add(new CargoItemsMonitor(
                    display: GetPairedBigDisplay("компоненты 1"),
                    containers: allContainers,
                    itemToMaxCount: Items.COMPONENTS.GetRange(0, Items.COMPONENTS.Count / 2).ToDictionary(item => item, item => 10000L),
                    headerText: "КОМПОНЕНТЫ",
                    progressbarSettings: PROGRESSBAR_SETTINGS
                ));

                result.Add(new CargoItemsMonitor(
                    display: GetPairedBigDisplay("компоненты 2"),
                    containers: allContainers,
                    itemToMaxCount: Items.COMPONENTS
                        .GetRange(Items.COMPONENTS.Count / 2, Items.COMPONENTS.Count - Items.COMPONENTS.Count / 2)
                        .ToDictionary(item => item, item => 10000L),
                    headerText: "КОМПОНЕНТЫ",
                    progressbarSettings: PROGRESSBAR_SETTINGS
                ));

                result.Add(new RefinersMonitor(
                    display: GetPairedBigDisplay("заводы"),
                    displayedOres: Items.ORES,
                    countRefinersByOreType: new Dictionary<Ore, int>
                    {
                        { Items.Ores.STONE, 1},
                        { Items.Ores.IRON, 1},
                        { Items.Ores.SILICON, 1},
                        { Items.Ores.NICKEL, 1},
                        { Items.Ores.COBALT, 1},
                        { Items.Ores.MAGNESIUM, 1},
                        { Items.Ores.SILVER, 1},
                        { Items.Ores.GOLD, 1},
                        { Items.Ores.PLATINUM, 1},
                        { Items.Ores.URANIUM, 1}
                    },
                    countUniversalRefiners: 0,
                    containers: allContainers,
                    headerText: "ОЧИСТИТЕЛЬНЫЕ ЗАВОДЫ"
                ));

                result.Add(new AssemblersMonitor(
                    display: GetPairedBigDisplay("сборщики"),
                    assemblers: allAssemblers,
                    headerText: "СБОРЩИКИ"
                ));

                var groupContainersByName = new Dictionary<string, List<IMyEntity>>();
                groupContainersByName.Add("Все", allContainers);
                result.Add(new CargoVolumeMonitor(
                    display: GetPairedBigDisplay("хранилища"),
                    groupEntityByName: groupContainersByName,
                    headerText: "ХРАНИЛИЩА",
                    progressbarSettings: PROGRESSBAR_SETTINGS
                ));

                var gasTankO2 = grid.GetBlockWithName("[GTW] Водородный бак") as IMyGasTank;
                var groupGasTanksByName = new Dictionary<string, List<IMyGasTank>>();
                groupGasTanksByName.Add("Водород", new List<IMyGasTank> { gasTankO2 });
                result.Add(new GasMonitor(
                    display: GetPairedBigDisplay("газы"),
                    groupEntityByName: groupGasTanksByName,
                    headerText: "ГАЗЫ",
                    progressbarSettings: PROGRESSBAR_SETTINGS
                ));

                var batteries = new List<IMyBatteryBlock>();
                grid.GetBlocksOfType(batteries);
                var groupBatteriesByName = Utils.GetBlocksByGridName(batteries);
                result.Add(new BatteryMonitor(
                    display: GetPairedBigDisplay("батареи"),
                    groupEntityByName: groupBatteriesByName,
                    headerText: "БАТАРЕИ",
                    progressbarSettings: PROGRESSBAR_SETTINGS
                ));

                return result;
            }

            private IDisplay GetDefaultDisplay(string name, int length)
            {
                return new Display(
                    textPanel: grid.GetBlockWithName(GRID_PREFIX + DISPLAY_PREFIX + name) as IMyTextPanel,
                    length: length,
                    fontSize: 51.0f / length,
                    fontColor: FONT_COLOR,
                    backgroundColor: BACKGROUND_COLOR
                );
            }

            private IDisplay GetBigDisplay(string name, int length)
            {
                return new Display(
                    textPanel: grid.GetBlockWithName(GRID_PREFIX + DISPLAY_PREFIX + name) as IMyTextPanel,
                    length: length,
                    fontSize: 25.5f / length,
                    fontColor: FONT_COLOR,
                    backgroundColor: BACKGROUND_COLOR
                );
            }

            private IDisplay GetPairedBigDisplay(string name, int length)
            {
                int padding = 2;
                return new DisplayGroup(
                    textPanels: new List<IMyTextPanel>() {
                        grid.GetBlockWithName(GRID_PREFIX + DISPLAY_PREFIX + name + " (1)") as IMyTextPanel,
                        grid.GetBlockWithName(GRID_PREFIX + DISPLAY_PREFIX + name + " (2)") as IMyTextPanel
                    },
                    length: length,
                    fontSize: 52.5f / (length + padding*2),
                    fontColor: FONT_COLOR,
                    backgroundColor: BACKGROUND_COLOR,
                    padding: padding
                );
            }

            private IDisplay GetDefaultDisplay(string name)
            {
                return GetDefaultDisplay(name, DISPLAY_SIZE);
            }

            private IDisplay GetBigDisplay(string name)
            {
                return GetBigDisplay(name, DISPLAY_SIZE);
            }

            private IDisplay GetPairedBigDisplay(string name)
            {
                return GetPairedBigDisplay(name, DISPLAY_SIZE);
            }
        }
    }
}
