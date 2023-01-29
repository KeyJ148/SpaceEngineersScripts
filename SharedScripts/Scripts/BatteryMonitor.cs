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
        public class BatteryMonitor : ProgressbarMonitor<IMyBatteryBlock>
        {
            public BatteryMonitor(IDisplay display, string headerText, ProgressbarSettings progressbarSettings,
                Dictionary<string, List<IMyBatteryBlock>> groupEntityByName) :
                base(display, headerText, progressbarSettings, groupEntityByName) 
            { }

            protected override long GetCurrentValue(IMyBatteryBlock battery)
            {
                return (long) ((double) battery.CurrentStoredPower * 1000000);
            }

            protected override long GetMaxValue(IMyBatteryBlock battery)
            {
                return (long)((double)battery.MaxStoredPower * 1000000);
            }
        }
    }
}
