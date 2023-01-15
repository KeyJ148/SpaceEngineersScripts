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
        public class GasMonitor : IMonitor
        {
            private readonly Display display;
            private readonly Dictionary<List<IMyGasTank>, string> groupTanksWithName;
            private readonly int maxNameLength;
            private readonly string headerText;
            private readonly ProgressbarSettings progressbarSettings;

            public GasMonitor(Display display, Dictionary<List<IMyGasTank>, string> groupTanksWithName, string headerText,
                ProgressbarSettings progressbarSettings)
            {
                this.display = display;
                this.groupTanksWithName = groupTanksWithName;
                this.headerText = headerText;
                maxNameLength = groupTanksWithName.Values.Select(name => name.Length).Max();
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

                groupTanksWithName.Keys.ToList()
                    .ForEach(containers => display.Println(GetGasInfoLine(containers, groupTanksWithName[containers])));
            }

            public string GetGasInfoLine(List<IMyGasTank> tanks, string name)
            {
                int countSpaceAfterName = maxNameLength - name.Length;
                long currentVolumeSum = tanks.Select(tank => (long) (tank.Capacity * tank.FilledRatio)).Sum();
                long maxVolumeSum = tanks.Select(tank => (long) tank.Capacity).Sum();

                return name + new string(' ', countSpaceAfterName) +
                    Utils.GetProgressBar(currentVolumeSum, maxVolumeSum, progressbarSettings);
            }
        }
    }
}
