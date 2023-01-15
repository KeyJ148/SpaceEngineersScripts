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
        public class CargoItemsMonitor : IMonitor
        {
            private readonly Display display;
            private readonly List<IMyEntity> containers;
            private readonly Dictionary<Item, long> itemToMaxCount;
            private readonly int maxNameLength;
            private readonly string headerText;
            private readonly ProgressbarSettings progressbarSettings;

            public CargoItemsMonitor(Display display, List<IMyEntity> containers, Dictionary<Item, long> itemToMaxCount, string headerText,
                ProgressbarSettings progressbarSettings)
            {
                this.display = display;
                this.containers = containers;
                this.headerText = headerText;
                this.itemToMaxCount = itemToMaxCount;
                maxNameLength = itemToMaxCount.Keys.Select(item => item.Name.Length).Max();
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

                List<IMyInventory> inventories = Utils.GetAllInventory(containers);
                List<Item> itemsToCalculate = itemToMaxCount.Keys.ToList();
                Dictionary<Item, long> itemsToCount = Utils.CalculateItemsInAllInventory(inventories, itemsToCalculate);

                itemsToCount.Keys.ToList().ForEach(item => display.Println(GetCargoInfoLine(item, itemsToCount[item])));
            }

            private String GetCargoInfoLine(Item item, long count)
            {
                int countSpaceAfterName = maxNameLength - item.Name.Length;
                return item.Name + new string(' ', countSpaceAfterName) +
                    Utils.GetProgressBar(count, itemToMaxCount[item], progressbarSettings);
            }
        }
    }
}
