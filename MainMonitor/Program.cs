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
    partial class Program : MyGridProgram
    {
        private const string GRID_PREFIX = "";
        private const string DISPLAY_PREFIX = "Дисплей - ";
        private const int DISPLAY_SIZE = 34;
        private readonly ProgressbarSettings PROGRESSBAR_SETTINGS = new ProgressbarSettings(' ', '■', '■', 0);

        private const long ORES_MAX_COUNT = 100000L;
        private const long INGOT_MAX_COUNT = ORES_MAX_COUNT * 3;

        private readonly List<IMonitor> monitors = new List<IMonitor>();

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            var allContainers = new List<IMyEntity>();
            GridTerminalSystem.GetBlocksOfType(allContainers);
            var allAssemblers = new List<IMyAssembler>();
            GridTerminalSystem.GetBlocksOfType(allAssemblers);

            monitors.Add(new CargoItemsMonitor(
                display: GetDefaultDisplay("руды"),
                containers: allContainers,
                itemToMaxCount: Items.ORES.ToDictionary(item => (Item) item, item => ORES_MAX_COUNT),
                headerText: "РУДЫ",
                progressbarSettings: PROGRESSBAR_SETTINGS
            ));

            monitors.Add(new CargoItemsMonitor(
                display: GetDefaultDisplay("слитки"),
                containers: allContainers,
                itemToMaxCount: Items.ORES.ToDictionary(ore => ore.Ingot, ore => (long) (ore.RefineEfficiency * INGOT_MAX_COUNT)),
                headerText: "СЛИТКИ",
                progressbarSettings: PROGRESSBAR_SETTINGS
             ));

            monitors.Add(new CargoItemsMonitor(
                display: GetDefaultDisplay("компоненты 1"),
                containers: allContainers,
                itemToMaxCount: Items.COMPONENTS.GetRange(0, Items.COMPONENTS.Count/2).ToDictionary(item => item, item => 10000L),
                headerText: "КОМПОНЕНТЫ",
                progressbarSettings: PROGRESSBAR_SETTINGS
            ));

            monitors.Add(new CargoItemsMonitor(
                display: GetDefaultDisplay("компоненты 2"),
                containers: allContainers,
                itemToMaxCount: Items.COMPONENTS.GetRange(Items.COMPONENTS.Count / 2, Items.COMPONENTS.Count - Items.COMPONENTS.Count / 2)
                    .ToDictionary(item => item, item => 10000L),
                headerText: "КОМПОНЕНТЫ",
                progressbarSettings: PROGRESSBAR_SETTINGS
            ));

            monitors.Add(new RefinersMonitor(
                display: GetDefaultDisplay("заводы"),
                displayedOres: Items.ORES,
                countRefinersByOreType: new Dictionary<Ore, int>(),
                countUniversalRefiners: 9,
                containers: allContainers,
                headerText: "ОЧИСТИТЕЛЬНЫЕ ЗАВОДЫ"
            ));

            monitors.Add(new AssemblersMonitor(
                display: GetDefaultDisplay("сборщики"),
                assemblers: allAssemblers,
                headerText: "СБОРЩИКИ"
            ));

            var container1 = GridTerminalSystem.GetBlockWithName("[BFM] Контейнер 1");
            var container2 = GridTerminalSystem.GetBlockWithName("[BFM] Контейнер 2");
            var groupContainersByName = new Dictionary<string, List<IMyEntity>>();
            groupContainersByName.Add("Все", allContainers);
            groupContainersByName.Add("Б. контейнеры", new List<IMyEntity> { container1, container2 });
            groupContainersByName.Add("Контейнер 1", new List<IMyEntity> { container1 });
            groupContainersByName.Add("Контейнер 2", new List<IMyEntity> { container2 });
            monitors.Add(new CargoVolumeMonitor(
                display: GetDefaultDisplay("хранилища"),
                groupEntityByName: groupContainersByName,
                headerText: "ХРАНИЛИЩА",
                progressbarSettings: PROGRESSBAR_SETTINGS
            ));

            var gasTankO2 = GridTerminalSystem.GetBlockWithName("[BFM] Водородный бак") as IMyGasTank;
            var groupGasTanksByName = new Dictionary<string, List<IMyGasTank>>();
            groupGasTanksByName.Add("Водород", new List<IMyGasTank> { gasTankO2 });
            monitors.Add(new GasMonitor(
                display: GetDefaultDisplay("газы"),
                groupEntityByName: groupGasTanksByName,
                headerText: "ГАЗЫ",
                progressbarSettings: PROGRESSBAR_SETTINGS
            ));

            var batteries = new List<IMyBatteryBlock>();
            GridTerminalSystem.GetBlocksOfType(batteries);
            var groupBatteriesByName = Utils.GetBlocksByGridName(batteries);
            monitors.Add(new BatteryMonitor(
                display: new Display(GridTerminalSystem.GetBlockWithName(GRID_PREFIX + DISPLAY_PREFIX + "батареи") as IMyTextPanel,
                    DISPLAY_SIZE * 2),
                groupEntityByName: groupBatteriesByName,
                headerText: "БАТАРЕИ",
                progressbarSettings: PROGRESSBAR_SETTINGS
            ));
        }

        public void Main(string argument, UpdateType updateSource)
        {
            foreach(var monitor in monitors)
            {
                monitor.Update();
                monitor.Render();
            }
        }

        private Display GetDefaultDisplay(string name)
        {
            return new Display(GridTerminalSystem.GetBlockWithName(GRID_PREFIX + DISPLAY_PREFIX + name) as IMyTextPanel, DISPLAY_SIZE);
        }
    }
}
