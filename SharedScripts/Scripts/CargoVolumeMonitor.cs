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
        public class CargoVolumeMonitor : IMonitor
        {

            private readonly Display display;
            private readonly Dictionary<List<IMyEntity>, string> groupContainersWithName;
            private readonly int maxNameLength;
            private readonly string headerText;
            private readonly ProgressbarSettings progressbarSettings;

            public CargoVolumeMonitor(Display display, Dictionary<List<IMyEntity>, string> groupContainersWithName, string headerText,
                ProgressbarSettings progressbarSettings)
            {
                this.display = display;
                this.groupContainersWithName = groupContainersWithName;
                this.headerText = headerText;
                maxNameLength = groupContainersWithName.Values.Select(name => name.Length).Max();
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

                groupContainersWithName.Keys.ToList()
                    .ForEach(containers => display.Println(GetVolumeInfoLine(containers, groupContainersWithName[containers])));
            }

            public string GetVolumeInfoLine(List<IMyEntity> containers, string name)
            {
                int countSpaceAfterName = maxNameLength - name.Length;

                List<IMyInventory> inventories = Utils.GetAllInventory(containers);
                long currentVolumeSum = inventories.Select(inventory => Utils.FromRaw(inventory.CurrentVolume.RawValue) * 1000).Sum();
                long maxVolumeSum = inventories.Select(inventory => Utils.FromRaw(inventory.MaxVolume.RawValue) * 1000).Sum();

                return name + new string(' ', countSpaceAfterName) +
                    Utils.GetProgressBar(currentVolumeSum, maxVolumeSum, progressbarSettings);
            }
        }
    }
}
