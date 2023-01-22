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
        public class RefinersMonitor : BasicMonitor<Ore, long>
        {
            private readonly List<IMyEntity> containers;

            private Dictionary<Item, long> displayedOresToCount;

            public RefinersMonitor(Display display, List<Ore> displayedOres, Dictionary<Ore, int> countRefinersByOreType,
                int countUniversalRefiners, List<IMyEntity> containers, string headerText) :
                base(display, headerText, displayedOres //Dictionary<Ore, long> ore -> speedInHour
                    .ToDictionary(ore => ore, ore => GetRefineSpeedInHour(ore, countRefinersByOreType, countUniversalRefiners)))
            {
                this.containers = containers;
            }

            public override void Update()
            {
                List<IMyInventory> inventories = Utils.GetAllInventory(containers);
                List<Item> itemsToCalculate = new List<Item>(monitoringEntities.Keys);
                displayedOresToCount = Utils.CalculateItemsInAllInventory(inventories, itemsToCalculate);
            }

            protected override string GetName(KeyValuePair<Ore, long> entity)
            {
                return entity.Key.Name;
            }

            protected override string GetInfo(KeyValuePair<Ore, long> entity)
            {
                Ore ore = entity.Key;
                long count = displayedOresToCount[ore];
                long speedInHour = entity.Value;
                long hours = count / speedInHour;

                return $" ({Utils.GetShortNumber(speedInHour, true)}/Час): {hours} {Utils.GetHourTranslate(hours)}";
            }

            private static long GetRefineSpeedInHour(Ore ore, Dictionary<Ore, int> countRefinersByOreType, int countUniversalRefiners)
            {
                long countRefiners = countUniversalRefiners + countRefinersByOreType.GetValueOrDefault(ore, 0);
                return (long) (countRefiners * ore.RefineSpeed * 3600);
            }
        }
    }
}
