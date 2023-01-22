﻿using Sandbox.Game.EntityComponents;
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
        private readonly List<IMonitor> monitors = new List<IMonitor>();

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            ProgressbarSettings progressbarSettings = new ProgressbarSettings(' ', '■', '■', 0);

            List<IMyEntity> allContainers = new List<IMyEntity>();
            GridTerminalSystem.GetBlocksOfType<IMyEntity>(allContainers);
            List<IMyAssembler> allAssemblers = new List<IMyAssembler>();
            GridTerminalSystem.GetBlocksOfType<IMyAssembler>(allAssemblers);

            monitors.Add(new CargoItemsMonitor(
                display: new Display(GridTerminalSystem.GetBlockWithName("Дисплей - руды") as IMyTextPanel, 34),
                containers: allContainers,
                itemToMaxCount: Items.ORES.ToDictionary(item => (Item) item, item => 100000L),
                headerText: "РУДЫ",
                progressbarSettings: progressbarSettings
            ));

            monitors.Add(new CargoItemsMonitor(
                 display: new Display(GridTerminalSystem.GetBlockWithName("Дисплей - слитки") as IMyTextPanel, 34),
                 containers: allContainers,
                 itemToMaxCount: Items.INGOTS.ToDictionary(item => item, item => 100000L),
                 headerText: "СЛИТКИ",
                progressbarSettings: progressbarSettings
             ));

            monitors.Add(new CargoItemsMonitor(
                display: new Display(GridTerminalSystem.GetBlockWithName("Дисплей - компоненты 1") as IMyTextPanel, 34),
                containers: allContainers,
                itemToMaxCount: Items.COMPONENTS.GetRange(0, Items.COMPONENTS.Count/2).ToDictionary(item => item, item => 10000L),
                headerText: "КОМПОНЕНТЫ",
                progressbarSettings: progressbarSettings
            ));

            monitors.Add(new CargoItemsMonitor(
                display: new Display(GridTerminalSystem.GetBlockWithName("Дисплей - компоненты 2") as IMyTextPanel, 34),
                containers: allContainers,
                itemToMaxCount: Items.COMPONENTS.GetRange(Items.COMPONENTS.Count / 2, Items.COMPONENTS.Count - Items.COMPONENTS.Count / 2)
                    .ToDictionary(item => item, item => 10000L),
                headerText: "КОМПОНЕНТЫ",
                progressbarSettings: progressbarSettings
            ));

            monitors.Add(new RefinersMonitor(
                display: new Display(GridTerminalSystem.GetBlockWithName("Дисплей - заводы") as IMyTextPanel, 34),
                displayedOres: Items.ORES,
                countRefinersByOreType: new Dictionary<Ore, int>(),
                countUniversalRefiners: 9,
                containers: allContainers,
                headerText: "ОЧИСТИТЕЛЬНЫЕ ЗАВОДЫ"
            ));

            monitors.Add(new AssemblersMonitor(
                display: new Display(GridTerminalSystem.GetBlockWithName("Дисплей - сборщики") as IMyTextPanel, 34),
                assemblers: allAssemblers,
                headerText: "СБОРЩИКИ"
            ));

            IMyEntity container1 = GridTerminalSystem.GetBlockWithName("[BFM] Контейнер 1");
            IMyEntity container2 = GridTerminalSystem.GetBlockWithName("[BFM] Контейнер 2");
            var groupContainersByName = new Dictionary<string, List<IMyEntity>>();
            groupContainersByName.Add("Все", allContainers);
            groupContainersByName.Add("Б. контейнеры", new List<IMyEntity> { container1, container2 });
            groupContainersByName.Add("Контейнер 1", new List<IMyEntity> { container1 });
            groupContainersByName.Add("Контейнер 2", new List<IMyEntity> { container2 });
            monitors.Add(new CargoVolumeMonitor(
                display: new Display(GridTerminalSystem.GetBlockWithName("Дисплей - хранилища") as IMyTextPanel, 34),
                groupEntityByName: groupContainersByName,
                headerText: "ХРАНИЛИЩА",
                progressbarSettings: progressbarSettings
            ));

            IMyGasTank gasTankO2 = GridTerminalSystem.GetBlockWithName("[BFM] Водородный бак") as IMyGasTank;
            var groupGasTanksByName = new Dictionary<string, List<IMyGasTank>>();
            groupGasTanksByName.Add("Водород", new List<IMyGasTank> { gasTankO2 });
            monitors.Add(new GasMonitor(
                display: new Display(GridTerminalSystem.GetBlockWithName("Дисплей - газы") as IMyTextPanel, 34),
                groupEntityByName: groupGasTanksByName,
                headerText: "ГАЗЫ",
                progressbarSettings: progressbarSettings
            ));

            List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();
            GridTerminalSystem.GetBlocksOfType(batteries);
            var groupBatteriesByName = Utils.GetBlocksByGridName(batteries);
            monitors.Add(new BatteryMonitor(
                display: new Display(GridTerminalSystem.GetBlockWithName("Дисплей - батареи") as IMyTextPanel, 68),
                groupEntityByName: groupBatteriesByName,
                headerText: "БАТАРЕИ",
                progressbarSettings: progressbarSettings
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
    }
}
