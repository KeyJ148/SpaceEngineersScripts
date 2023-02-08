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
    /// <summary>
    /// TODO: Балансировщик компонентов. Автоматически запускает крафт компонента, когда его количество
    /// падает ниже определённого уровня
    /// </summary>
    partial class Program : MyGridProgram
    {
        // CONFIG
        private int _tier = 1; // степень 10, на которую будет умножено требуемое количество предметов
        private bool _limitByGroup = true; // Если true, то будет использовать главные сборщики в группе _groupName
        private string _groupName = "1. Assemblers"; // см. _limitByGroup. Группа не должна содержать сборщиков в совместном режиме

        // Список предметов для дозаказа
        private readonly Dictionary<CraftableItem, long> _targetAmount = new Dictionary<CraftableItem, long>
        {
            { Items.Components.STEEL_PLATE, 1000 },
            { Items.Components.INTEROR_PLATE, 1000},
            { Items.Components.COMPUTER, 200 },
            { Items.Components.CONSTRUCTION, 200 },
            { Items.Components.SMALL_TUBE, 1500 },
            { Items.Components.GIRDER, 750 },
            { Items.Components.LARGE_TUBE, 100 },
            { Items.Components.METAL_GRID, 300 },
            { Items.Components.MOTOR, 750 },
            { Items.Components.SOLAR_CELL, 50 },
            { Items.Components.REACTOR, 100 },
            { Items.Components.THRUST, 100 },
            { Items.Components.POWER_CELL, 100 },
            { Items.Components.MEDICAL, 3 },
            { Items.Components.GRAVITY_GENERATOR, 20 },
            { Items.Components.RADIO_COMMUNICATION, 20 },
            { Items.Components.BULLETPROOF_GLASS, 100 },
            { Items.Components.DETECTOR, 50 },
            { Items.Components.DISPLAY, 150 },
            { Items.Components.EXPLOSIVES, 100 },
            { Items.Components.SUPERCONDUCTOR, 20}
        };


        // SCRIPT
        private List<IMyAssembler> _assemblers, _allAssemblers;
        private List<IMyProductionBlock> _allProducers;
        private List<IMyCargoContainer> _allContainers;
        private List<IMyInventory> _storages;

        public Program()
        {
            // Инициализация
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
            ScanGrid();

            // Подготовка задач
            Scheduler.ExecuteEveryNTicks(ScanGrid, 4).SetName("Scan Grid");
            Scheduler.ExecuteEveryNTicks(CheckStorage, 1).SetName("Check Storage");

            // Запуск
            Echo($"Finished scan!\n" +
                $"Total assemblers: {_allAssemblers.Count}\n" +
                $"Total containers: {_allContainers.Count}\n" +
                $"Total producers: {_allProducers.Count}\n" +
                $"Working assemblers: {_assemblers.Count}");
            Scheduler.Start();
        }

        /// <summary>
        /// Сканирует систему и собирает все ссылки на нужные объекты (контейнеры, ассемблеры)
        /// </summary>
        private void ScanGrid()
        {
            _allAssemblers = new List<IMyAssembler>();
            _allContainers = new List<IMyCargoContainer>();
            _allProducers = new List<IMyProductionBlock>();
            _assemblers = new List<IMyAssembler>();

            GridTerminalSystem.GetBlocksOfType(_allAssemblers);
            GridTerminalSystem.GetBlocksOfType(_allContainers);
            GridTerminalSystem.GetBlocksOfType(_allProducers);

            if (!_limitByGroup)
            {
                _assemblers = _allAssemblers.Where(a => !a.CooperativeMode && a.UseConveyorSystem).ToList(); // Все сборщики без совместного режима
            }
            else
            {
                GridTerminalSystem.GetBlockGroupWithName(_groupName).GetBlocksOfType(_assemblers);
            }
            _storages = _allContainers.Select(c => c.GetInventory())
                .Concat(_allProducers.Select(p => p.OutputInventory)).ToList(); // Инвентари контейнеров*/
        }

        /// <summary>
        /// проверяет количество предметов в системе (с учетом тех, что заказаны в сборщиках), и заказывает,
        /// если их не достаточно
        /// </summary>
        private void CheckStorage()
        {
            var itemsAmount = CountItems(); // словарь, где каждому предмету соответствует его количество в системе
            bool first = true;
            foreach (var item in _targetAmount.Keys)
            {
                // разница между требуемым и имеющимся количеством
                long diff = (_targetAmount[item] * (long)Math.Pow(10, _tier - 1)) - itemsAmount[item]; 
                if (diff > 0)
                {
                    if (first)
                    {
                        Echo("Enqueue requured: ");
                        first = false;
                    }
                    Echo($"{diff} {item.Id.SubtypeName}");
                    EnqueueItem(item, diff);
                }
            }
        }

        /// <summary>
        /// Ставит предмет в очередь крафта
        /// </summary>
        /// <param name="item">Требуемый предмет</param>
        /// <param name="amount">Сколько накрафтить</param>
        private void EnqueueItem(CraftableItem item, long amount)
        {
            try
            {
                var blueprint = item.BlueprintId;
                var assembler = Utils.GetLeastLoadedAssembler(_assemblers.Where(a => a.CanUseBlueprint(blueprint)).ToList());
                assembler.AddQueueItem(blueprint, Utils.ToFixedPoint(amount));
            }
            catch (Exception e){
                Echo(e.Message);
            }
        }

        /// <summary>
        /// Считает количество предметов в системе
        /// </summary>
        /// <returns></returns>
        private Dictionary<CraftableItem, long> CountItems()
        {
            var itemsAmount = _targetAmount.Keys.ToDictionary(key => key, key => 0L);
            // Ты не можешь просто итерироваться по словарю и менят ь ему значения. Всё сложно
            var keys = itemsAmount.Keys.ToList(); 
            try
            {
                foreach (var item in keys)
                {
                    // Подсчет предметов в контейнерах
                    itemsAmount[item] += _storages.Sum(storage => Utils.CalculateItemsInInventory(item, storage));
                    // Подсчет предметов, заказанных в сборщиках
                    itemsAmount[item] += _allAssemblers.Sum(assembler => Utils.CalculateQueuedItems(item, assembler));
                }
            }
            catch (Exception e)
            {
                Echo(e.Message);
            }
            return itemsAmount;
        }

        public void Main(string argument, UpdateType updateSource)
        { 
            Scheduler.Tick();
        }
    }
}
