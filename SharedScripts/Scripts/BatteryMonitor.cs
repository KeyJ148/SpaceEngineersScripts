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
        public class BatteryMonitor : IMonitor
        {
            private readonly Display display;
            private readonly Dictionary<List<IMyBatteryBlock>, string> groupBatteriesWithName;
            private readonly int maxNameLength;
            private readonly string headerText;
            private readonly ProgressbarSettings progressbarSettings;

            public BatteryMonitor(Display display, Dictionary<List<IMyBatteryBlock>, string> groupBatteriesWithName, string headerText,
                ProgressbarSettings progressbarSettings)
            {
                this.display = display;
                this.groupBatteriesWithName = groupBatteriesWithName;
                this.headerText = headerText;
                maxNameLength = groupBatteriesWithName.Values.Select(name => name.Length).Max();
                this.progressbarSettings = new ProgressbarSettings(
                    progressBarEmpty: progressbarSettings.ProgressBarEmpty,
                    progressbarFull: progressbarSettings.ProgressbarFull,
                    progressBar100percent: progressbarSettings.ProgressBar100percent,
                    lenght: display.Length - maxNameLength);
            }

            public void Render()
            {
                display.Clear();
                if (headerText != null)
                {
                    display.PrintMiddle(headerText);
                }

                groupBatteriesWithName.Keys.ToList()
                    .ForEach(gridName => display.Println(GetBatteryInfoLine(gridName, groupBatteriesWithName[gridName])));
            }

            public string GetBatteryInfoLine(List<IMyBatteryBlock> batteries, string name)
            {
                int countSpaceAfterName = maxNameLength - name.Length;
                long currentPowerSum = batteries.Select(battery => (long) ((double) battery.CurrentStoredPower * 1000000)).Sum();
                long maxPowerSum = batteries.Select(battery => (long)((double) battery.MaxStoredPower * 1000000)).Sum();

                return name + new string(' ', countSpaceAfterName) +
                    Utils.GetProgressBar(currentPowerSum, maxPowerSum, progressbarSettings);
            }
        }
    }
}
