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

            /// <summary>
            /// Проклятый метод
            /// </summary>
            /// <param name="num"></param>
            /// <returns></returns>
            public static MyFixedPoint ToFixedPoint(long num)
            {
                var fixedPoint = new MyFixedPoint();
                fixedPoint.RawValue = num * 1000000;
                return fixedPoint;
            }

            /// <summary>
            /// Возвращает сокращенное число с суффиксом
            /// TODO: Женя, напиши че тут происходит и зачем
            /// </summary>
            /// <param name="number"></param>
            /// <param name="alignment"></param>
            /// <returns></returns>
            public static string GetShortNumber(long number, bool alignment)
            {
                char[] suffixes = { ' ', 'K', 'M', 'G', 'T', 'P', 'E', '?' };
                int numberLen = number.ToString().Length;

                //number = numberBase * 10 ^ (3 * numberPower3)
                int numberPower3 = (numberLen - 1) / 3;
                long numberBase = number / (long)Math.Pow(10, numberPower3 * 3);
                int suffixIndex = Math.Min(numberPower3, suffixes.Length - 1);
                char suffix = suffixes[suffixIndex];

                string prefix = alignment ? new string(' ', 3 - numberBase.ToString().Length) : "";
                return prefix + numberBase.ToString() + suffix;
            }

            /// <summary>
            /// Возвращает правильное написание "часа" в зависимости от числа
            /// </summary>
            /// <param name="hours"></param>
            /// <returns></returns>
            public static string GetHourTranslate(long hours)
            {
                long val1 = hours % 10;
                long val10 = (hours % 100) / 10;

                if (val10 != 1 && val1 == 1)
                {
                    return "час";
                }
                if (val10 != 1 && val1 >= 2 && val1 <= 4)
                {
                    return "часа";
                }
                return "часов";
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

            /// <summary>
            /// Возвращает количество искомых предметов в указанном инвентаре
            /// </summary>
            /// <param name="item">Искомый предмет</param>
            /// <param name="inventory">Инвентарь</param>
            /// <returns></returns>
            public static long CalculateItemsInInventory(Item item, IMyInventory inventory)
            {
                return FromRaw(inventory.GetItemAmount(item.Id).RawValue);
            }

            /// <summary>
            /// Возвращает количество заказанных предметов в сборщике
            /// </summary>
            /// <param name="item">Искомый предмет</param>
            /// <param name="assembler">Сборщик</param>
            /// <returns></returns>
            public static long CalculateQueuedItems(Item item, IMyAssembler assembler)
            {
                var amount = 0L;
                var queue = new List<MyProductionItem>();
                assembler.GetQueue(queue);
                foreach (var queuedItem in queue)
                {
                    if (queuedItem.BlueprintId.SubtypeName == GetBlueprintSubdefinitionName(item.Id.SubtypeName))
                    {
                        amount += FromRaw(queuedItem.Amount.RawValue);
                    }
                }

                return amount;
            }

            /// <summary>
            /// Возвращает название чертежа, соответствующего названию предмета
            /// </summary>
            /// <param name="itemSubdefinition"></param>
            /// <returns></returns>
            public static string GetBlueprintSubdefinitionName(string itemSubdefinition)
            {
                var suffix = "Component";
                var subdefs = new[] { "Motor", "Computer", "Construction", "Detector", "Explosives", "Girder",
                    "GravityGenerator", "Medical", "Thrust", "RadioCommunication", "Reactor" };

                if (subdefs.Contains(itemSubdefinition))
                    return itemSubdefinition + suffix;

                return itemSubdefinition;
            }

            /// <summary>
            /// Возвращает сборщик из предоставленного списка с наименьшим ETA
            /// </summary>
            /// <param name="assemblers">Список сборщиков</param>
            /// <returns></returns>
            public static IMyAssembler GetLeastLoadedAssembler(List<IMyAssembler> assemblers)
            {
                IMyAssembler min;
                try
                {
                    min = assemblers.MinBy(assembler => {
                        return (float)GetAssemblerEta(assembler);
                    });
                }
                catch(Exception e)
                {
                    return assemblers.First();
                }
                return min;
            }

            /// <summary>
            /// Рассчитывает суммарное время завершения всех заказов в сборщике
            /// </summary>
            /// <param name="assembler"></param>
            /// <returns></returns>
            public static double GetAssemblerEta(IMyAssembler assembler)
            {
                double eta = 0;
                var queue = new List<MyProductionItem>();
                assembler.GetQueue(queue);
                foreach (var queuedItem in queue)
                {
                    eta += Utils.GetBlueprintResult(queuedItem).CraftingTime * Utils.FromRaw(queuedItem.Amount.RawValue);
                }
                return eta;
            }

            /// <summary>
            /// Возвращает ссылку на предмет, являющийся результатом сборки чертежа
            /// </summary>
            /// <param name="prodItem"></param>
            /// <returns></returns>
            public static CraftableItem GetBlueprintResult(MyProductionItem prodItem)
            {
                var items = Items.CRAFTABLES.Where(item => item.Id.SubtypeName == prodItem.BlueprintId.SubtypeName
                .Replace("Component",""));
                if (items.Count() > 0)
                    return items.First();

                return null;
            }


            public static string GetGridName(IMyCubeBlock block)
            {
                return block.CubeGrid.CustomName;
            }

            public static Dictionary<string, List<T>> GetBlocksByGridName<T>(List<T> blocks) where T : IMyCubeBlock
            {
                return blocks
                    .GroupBy(block => GetGridName(block))
                    .ToDictionary(group => group.Key, group => group.ToList());
            }
        }
    }
}
