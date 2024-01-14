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
        public class CargoItemsMonitor : BasicMonitor<Item, long>
        {
            private readonly List<IMyEntity> containers;
            private readonly ProgressbarSettings progressbarSettings;

            private Dictionary<Item, long> itemsToCount;

            public CargoItemsMonitor(IDisplay display, string headerText, ProgressbarSettings progressbarSettings,
                List<IMyEntity> containers, Dictionary<Item, long> itemToMaxCount) :
                base(display, headerText, itemToMaxCount)
            {
                this.containers = containers; 
                this.progressbarSettings = new ProgressbarSettings(
                     progressBarEmpty: progressbarSettings.ProgressBarEmpty,
                     progressbarFull: progressbarSettings.ProgressbarFull,
                     progressBar100percent: progressbarSettings.ProgressBar100percent,
                     length: display.GetLength() - maxNameLength);
            }

            public override void Update()
            {
                List<IMyInventory> inventories = Utils.GetAllInventory(containers);
                List<Item> itemsToCalculate = monitoringEntities.Keys.ToList();
                itemsToCount = Utils.CalculateItemsInAllInventory(inventories, itemsToCalculate);
            }

            protected override string GetName(KeyValuePair<Item, long> entity)
            {
                return entity.Key.Name;
            }

            protected override string GetInfo(KeyValuePair<Item, long> entity)
            {
                Item item = entity.Key;
                long maxCount = entity.Value;
                return Utils.GetProgressBar(itemsToCount[item], maxCount, progressbarSettings);
            }
        }
    }
}
