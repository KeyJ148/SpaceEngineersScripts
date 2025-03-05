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
        private readonly IMonitorSetup CURRENT_SETUP = new Vein11_Setup(); //Change for new ship
        private const int RECREATE_EVERY_TICKS = 60 * 10; //10 seconds
        
        private readonly MonitorCreator monitorCreator;
        private List<IMonitor> monitors = new List<IMonitor>();
        private int tickCounter = 0;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            IMonitorSetup configGenerator = CURRENT_SETUP; 
            MonitorCreatorConfig config = configGenerator.setup(GridTerminalSystem);
            monitorCreator = new MonitorCreator(GridTerminalSystem, config);
        }

        public void Main(string argument, UpdateType updateSource)
        {

            if (tickCounter <= 0)
            {
                monitors = monitorCreator.CreateAllMonitors();
                tickCounter = RECREATE_EVERY_TICKS;
            } 
            else
            {
                switch (updateSource)
                {
                    case UpdateType.Update1:
                        tickCounter -= 1; break;
                    case UpdateType.Update10:
                        tickCounter -= 10; break;
                    case UpdateType.Update100:
                        tickCounter -= 100; break;
                }
            }

            foreach (var monitor in monitors)
            {
                monitor.Update();
                monitor.Render();
            }
        }
    }
}
