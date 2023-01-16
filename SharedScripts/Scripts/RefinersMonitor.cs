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

            private readonly Display display;
            private readonly Dictionary<Ore, long> displayedOresToSpeedInHour;
            private readonly List<IMyEntity> containers;
            private readonly int maxNameLength;
            private readonly string headerText;

            public RefinersMonitor(Display display, List<Ore> displayedOres, Dictionary<Ore, int> countRefinersByOreType,
                int countUniversalRefiners, List<IMyEntity> containers, string headerText)
            {
                this.display = display;
                this.containers = containers;
                this.headerText = headerText;
                displayedOresToSpeedInHour = displayedOres
                    .ToDictionary(ore => ore, ore => GetRefineSpeedInHour(ore, countRefinersByOreType, countUniversalRefiners));
                maxNameLength = displayedOres.Select(ore => ore.Name.Length).Max();
            }

            public void Render()
            {
                display.Clear();
                if (headerText != null)
                {
                    display.PrintMiddle(headerText);
                }

                List<IMyInventory> inventories = Utils.GetAllInventory(containers);
                List<Item> itemsToCalculate = new List<Item>(displayedOresToSpeedInHour.Keys);
                Dictionary<Item, long> displayedOresToCount = Utils.CalculateItemsInAllInventory(inventories, itemsToCalculate);

                displayedOresToSpeedInHour.Keys.ToList().ForEach(ore => display.Println(GetSpeedInfoLine(ore, displayedOresToCount[ore])));
            }

            private long GetRefineSpeedInHour(Ore ore, Dictionary<Ore, int> countRefinersByOreType, int countUniversalRefiners)
            {
                long countRefiners = countUniversalRefiners + countRefinersByOreType.GetValueOrDefault(ore, 0);
                return (long) (countRefiners * ore.RefineSpeed * 3600);
            }

            private String GetSpeedInfoLine(Ore ore, long count)
            {
                int countSpaceAfterName = maxNameLength - ore.Name.Length;
                long speedInHour = displayedOresToSpeedInHour[ore];
                long hours = count / speedInHour;

                return $"{ore.Name}{new string(' ', countSpaceAfterName)} " +
                    $"({Utils.GetShortNumber(speedInHour, true)}/Час): {hours} {Utils.GetHourTranslate(hours)}";
            }
        }
    }
}
