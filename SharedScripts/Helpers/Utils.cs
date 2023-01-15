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
        public static class Utils
        {

			public static long FromRaw(long rawValue)
            {
				return rawValue / 1000000;
            }

			public static string GetShortNumber(long number, bool alignment)
			{
				char[] suffixes = {' ', 'K', 'M', 'G', 'T', 'P', 'E', '?'};
				int numberLen = number.ToString().Length;

				//number = numberBase * 10 ^ (3 * numberPower3)
				int numberPower3 = (numberLen - 1) / 3;
				long numberBase = number / (long) Math.Pow(10, numberPower3 * 3);
				int suffixIndex = Math.Min(numberPower3, suffixes.Length - 1);
				char suffix = suffixes[suffixIndex];

				string prefix = alignment ? new string(' ', 3 - numberBase.ToString().Length) : "";
				return prefix + numberBase.ToString() + suffix;
			}

			public static string GetHourTranslate(long hours)
            {
				long val1 = hours % 10;
				long val10 = (hours % 100) / 10;

				if (val10 != 1 && val1 == 1)
				{
					return "час";
				}
				if (val10 != 1 && val1 >= 2 && val1 <= 4) {
					return "часа";
				}
				return "часов";
			}

			public static string GetProgressBar(long count, long maxCount, ProgressbarSettings progressbarSettings)
            {
				const int COUNT_SPEC_CHARS = 13; // " [_____] 100K/999K" Number of chars without progress bar (symbols _____)

				int lenght = progressbarSettings.Lenght - COUNT_SPEC_CHARS;
				double percent = maxCount != 0 ? (double) count / maxCount : 0;
				int countCharFull = (int) Math.Floor(lenght * percent);
				int countCharEmpty = lenght - countCharFull;

				string progressbarChars = countCharEmpty > 0 ?
					new string(progressbarSettings.ProgressbarFull, countCharFull) +
						new string(progressbarSettings.ProgressBarEmpty, countCharEmpty) :
					new string(progressbarSettings.ProgressBar100percent, lenght);
				return $" [{progressbarChars}] {GetShortNumber(count, true)}/{GetShortNumber(maxCount, true)}";
			}

			public static List<IMyInventory> GetAllInventory(IMyEntity entity)
            {
				List<IMyInventory> inventories = new List<IMyInventory>(entity.InventoryCount);
                for (int i = 0; i < entity.InventoryCount; i++)
                {
					inventories.Add(entity.GetInventory(i));
                }

				return inventories;
            }

			public static List<IMyInventory> GetAllInventory(List<IMyEntity> entities)
			{
				return entities.SelectMany(GetAllInventory).ToList();
			}

			public static Dictionary<Item, long> CalculateItemsInAllInventory(List<IMyInventory> inventories, List<Item> itemsToCalculate)
            {
				Dictionary<Item, long> itemToCount = itemsToCalculate.ToDictionary(item => item, item => 0L);
				foreach (IMyInventory inventory in inventories)
				{
					itemsToCalculate.ForEach(item => itemToCount[item] += FromRaw(inventory.GetItemAmount(item.Id).RawValue));
				}
				return itemToCount;
			}
		}
    }
}
