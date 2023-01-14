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
        private readonly List<IMonitor> monitors = new List<IMonitor>();

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            List<IMyEntity> allContainers = new List<IMyEntity>();
            GridTerminalSystem.GetBlocksOfType<IMyEntity>(allContainers);
            List<IMyAssembler> allAssemblers = new List<IMyAssembler>();
            GridTerminalSystem.GetBlocksOfType<IMyAssembler>(allAssemblers);

            monitors.Add(new RefinersMonitor(
                display: new Display(GridTerminalSystem.GetBlockWithName("Дисплей 1") as IMyTextPanel, 35),
                displayedOres: Items.ORES,
                countRefinersByOreType: new Dictionary<Ore, int>(),
                countUniversalRefiners: 3,
                containers: allContainers,
                headerText: "ОЧИСТИТЕЛЬНЫЕ ЗАВОДЫ"
            ));

            monitors.Add(new AssemblersMonitor(
                display: new Display(GridTerminalSystem.GetBlockWithName("Дисплей 2") as IMyTextPanel, 35),
                assemblers: allAssemblers,
                headerText: "СБОРЩИКИ"
            ));
        }

        public void Main(string argument, UpdateType updateSource)
        {
            monitors.ForEach(monitor => monitor.Render());
        }
    }
}
