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
        public class RefinersMonitor : IMonitor
        {

            private readonly Display Display;
            private readonly Dictionary<Ore, long> DisplayedOresToSpeedInHour;
            private readonly List<IMyEntity> Containers;
            private readonly int MaxNameLength;
            private readonly string HeaderText;

            private RefinersMonitor(Display display, List<Ore> displayedOres, Dictionary<Ore, int> countRefinersByOreType,
                int countUniversalRefiners, List<IMyEntity> containers, string headerText)
            {
                Display = display;
                Containers = containers;
                HeaderText = headerText;
                DisplayedOresToSpeedInHour = displayedOres
                    .ToDictionary(ore => ore, ore => GetRefineSpeedInHour(ore, countRefinersByOreType, countUniversalRefiners));
                MaxNameLength = displayedOres.Select(ore => ore.Name.Length).Max();
            }

            public void Render()
            {
                Display.Clear();
                if (HeaderText != null)
                {
                    Display.PrintMiddle(HeaderText);
                }

                Dictionary<Ore, long> displayedOresToCount = DisplayedOresToSpeedInHour.Keys.ToDictionary(ore => ore, ore => 0L);
                foreach (IMyInventory inventory in Utils.GetAllInventory(Containers))
                {
                    displayedOresToCount.Keys.ToList().ForEach(ore =>
                        displayedOresToCount[ore] += inventory.GetItemAmount(ore.Id).ToIntSafe());
                }

                DisplayedOresToSpeedInHour.Keys.ToList().ForEach(ore => Display.Println(GetSpeedInfoLine(ore, displayedOresToCount[ore])));
            }

            private long GetRefineSpeedInHour(Ore ore, Dictionary<Ore, int> countRefinersByOreType, int countUniversalRefiners)
            {
                long countRefiners = countUniversalRefiners + countRefinersByOreType.GetValueOrDefault(ore, 0);
                return countRefiners * ore.RefineSpeed;
            }

            private String GetSpeedInfoLine(Ore ore, long count)
            {
                int countSpaceAfterName = MaxNameLength - ore.Name.Length;
                long speedInHour = DisplayedOresToSpeedInHour[ore];
                long hours = count / speedInHour;

                StringBuilder sb = new StringBuilder();
                sb.Insert(0, ore.Name);
                sb.Append(' ', countSpaceAfterName);
                sb.Append(" (");
                sb.Append(Utils.GetShortNumber(speedInHour, true));
                sb.Append("/Час): ");
                sb.Append(hours);
                sb.Append(" ");
                sb.Append(Utils.GetHourTranslate(hours));

                return sb.ToString();
            }

            public class Builder
            {

            }
        }
    }
}
