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
        public class GasMonitor : ProgressbarMonitor<IMyGasTank>
        {

            public GasMonitor(Display display, string headerText, ProgressbarSettings progressbarSettings,
                Dictionary<string, List<IMyGasTank>> groupEntityByName) :
                base(display, headerText, progressbarSettings, groupEntityByName) 
            { }

            protected override long GetCurrentValue(IMyGasTank gasTank)
            {
                return (long) (gasTank.Capacity * gasTank.FilledRatio);
            }

            protected override long GetMaxValue(IMyGasTank gasTank)
            {
                return (long) gasTank.Capacity;
            }
        }
    }
}
