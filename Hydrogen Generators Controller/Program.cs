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
        private IMyGridTerminalSystem gridTerminalSystem;
        private IMyCubeGrid cubeGrid;
        private GridMonitor Monitor;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;

            gridTerminalSystem = GridTerminalSystem;
            cubeGrid = Me.CubeGrid;

            Monitor = new GridMonitor(gridTerminalSystem, cubeGrid);
            Monitor.RescanGrid();

            Scheduler.ExecuteEveryNTicks(Monitor.RescanGrid, 60*5);

            Scheduler.Start();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            Scheduler.Tick();
        }
    }
}
