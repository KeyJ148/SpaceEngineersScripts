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
using VRage.Game.ModAPI;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;
using IMyCubeBlock = VRage.Game.ModAPI.Ingame.IMyCubeBlock;
using IMyInventory = VRage.Game.ModAPI.Ingame.IMyInventory;
using IMyInventoryItem = VRage.Game.ModAPI.Ingame.IMyInventoryItem;

namespace IngameScript
{
    /// <summary>
    /// Переносит предметики туда, где им место, для удобного доступа. Настройки в разделе Config позволяют настроить
    /// названия контейнеров, в которые будут складываться предметы.
    /// TODO: Протестировать возможность использовать группы контейнеров в качестве целевых хранилищ
    /// TODO: Создать поддержку правил выбора целевых контейнеров
    /// </summary>
    partial class Program : MyGridProgram
    {
        // Config
        private string
            _componentsName = "0. Components",
            _ingotsName = "0. Ingots",
            _oresName = "0. Ingots",
            _toolsName = "0. Tools";


        // Script
        private IMyGridTerminalSystem _grid; // ссылка на систему
        private List<IMyProductionBlock> _productors; // Список блоков-крафтеров
        private List<IMyCargoContainer> _containers; // Список всех контейнеров
        private List<Sorter> _sorters; // Виртуальные сортировщики
        public static Program Instance; // Ссылка на скрипт (для вызова Instance.Echo)

        public Program()
        {
            // Инициализация
            Instance = this;
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            _grid = GridTerminalSystem;
            Rescan();

            // Подготовка планировщика
            Scheduler.ExecuteEveryNTicks(Rescan,60*5);
            Scheduler.ExecuteEveryNTicks(TrySort,10);
            // Scheduler.UseLogAction(Echo); // Раскомментируй, чтобы планировщик начал срать тебе в консоль

            // Запуск планировщика
            Scheduler.Start();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            Scheduler.Tick();
        }

        /// <summary>
        /// Производит повторное сканирование всех блоков в системе. Очень ресурсоёмко.
        /// </summary>
        private void Rescan()
        {
            // Обновление кэша
            var blocks = GetLocalGridBlocks();
            _productors = new List<IMyProductionBlock>();
            _containers = new List<IMyCargoContainer>();
            _sorters = new List<Sorter>();

            // Целевые хранилища
            var _ingotsStore = (IMyCargoContainer) _grid.GetBlockWithName(_ingotsName);
            var _componentsStore = (IMyCargoContainer)_grid.GetBlockWithName(_componentsName);
            var _oresStore = (IMyCargoContainer)_grid.GetBlockWithName(_oresName);
            var _toolsStore = (IMyCargoContainer)_grid.GetBlockWithName(_toolsName);

            // Поиск сканируемых сортировщиками блоков
            _productors = blocks.OfType<IMyProductionBlock>().ToList();
            _containers = blocks.OfType<IMyCargoContainer>().ToList();

            // Подготовка виртуальных сортировщиков
            _sorters.Add(new Sorter(_ingotsStore, Items.INGOTS));
            _sorters.Add(new Sorter(_componentsStore, Items.COMPONENTS));
            _sorters.Add(new Sorter(_oresStore, new List<Item>(Items.ORES)));
            _sorters.Add(new Sorter(_oresStore, Items.INSTRUMENTS.Concat(Items.OTHER).ToList()));
        }

        /// <summary>
        /// Пытается перенести все предметы туда, куда этого хотят сортировщики
        /// </summary>
        private void TrySort()
        {
            foreach (var sorter in _sorters)
            {
                foreach (var container in _containers)
                {
                    sorter.TryExtract(container.GetInventory());
                }

                foreach (var productor in _productors)
                {
                    sorter.TryExtract(productor.OutputInventory);
                }
            }
        }

        /// <summary>
        /// Возвращает список блоков, содержащихся в локальной системе (без учета кораблей/коннекторов)
        /// </summary>
        /// <returns></returns>
        private List<IMyTerminalBlock> GetLocalGridBlocks()
        {
            var list = new List<IMyTerminalBlock>();
            _grid.GetBlocksOfType(list);
            list = list.Where(block => block.IsSameConstructAs(Me)).ToList();

            return list;
        }


        /// <summary>
        /// Виртуальный сортировщик. Пока что умеет только складывать по вайтлисту в один конкретный контейнер (инвентарь)
        /// </summary>
        private class Sorter
        {
            IMyInventory to;
            List<Item> _items;
            List<IMyInventory> toList;

            /// <summary>
            /// Создаёт виртуальный сортировщик
            /// </summary>
            /// <param name="target">Целевой инвентарь, в который будут складываться предметы</param>
            /// <param name="items">Список предметов, которые переносятся сортировщиком</param>
            public Sorter(IMyInventory target, List<Item> items)
            {
                to = target;
                _items = items;
            }

            /// <summary>
            /// Создаёт виртуальный сортировщик
            /// </summary>
            /// <param name="target">Целевой контейнер, в который будут складываться предметы</param>
            /// <param name="items">Список предметов, которые переносятся сортировщиком</param>
            public Sorter(IMyCargoContainer target, List<Item> items)
            {
                to = target.GetInventory();
                _items = items;
            }

            public Sorter(List<IMyCargoContainer> targets, List<Item> items)
            {
                foreach (var target in targets)
                {
                    toList.Add(target.GetInventory());
                }

                _items = items;
            }

            public Sorter(List<IMyInventory> targets, List<Item> items)
            {
                toList = targets;
                _items = items;
            }

            /// <summary>
            /// Пытается перенести все требуемые предметы из указанного инвентаря
            /// </summary>
            /// <param name="from">Инвентарь для сканирования</param>
            public void TryExtract(IMyInventory from)
            {
                try
                {
                    if (toList == null) // Для одиночного указанного контейнера
                    {
                        // Не пытаться сортировать самого себя
                        if (from == to)
                            return;

                        // проверка конвейерного соединения
                        if (!from.IsConnectedTo(to))
                            return;

                        List<MyInventoryItem> items = new List<MyInventoryItem>();
                        from.GetItems(items);

                        foreach (var item in items)
                        {
                            // Попытка найти предмет прямым перебором. Просто не люблю излишнюю вложенность.
                            var found = _items.Where(i => i.Id.ToString() == item.Type.ToString());

                            // Если нашелся хотя бы один - всё норм.
                            if (found.Count() <= 0)
                                continue;

                            // Не знаю что точно оно проверяет, но вроде полезно
                            if (!from.CanTransferItemTo(to, item.Type))
                                continue;

                            try // паранойя
                            {
                                from.TransferItemTo(to, item); // Двигаем штучки
                            }
                            catch { } // "ловить" поменбше
                        }
                    }
                    else // для списка контейнеров.
                    {

                        foreach (var target in toList)
                        {
                            // Не пытаться сортировать самого себя
                            if (from == to)
                                continue;

                            // проверка конвейерного соединения
                            if (!from.IsConnectedTo(target))
                                continue;

                            List<MyInventoryItem> items = new List<MyInventoryItem>();
                            from.GetItems(items);

                            foreach (var item in items)
                            {
                                // Попытка найти предмет прямым перебором. Просто не люблю излишнюю вложенность.
                                var found = _items.Where(i => i.Id.ToString() == item.Type.ToString());

                                // Если нашелся хотя бы один - всё норм.
                                if (found.Count() <= 0)
                                    continue;

                                // Не знаю что точно оно проверяет, но вроде полезно
                                if (!from.CanTransferItemTo(to, item.Type))
                                    continue;

                                try // паранойя
                                {
                                    from.TransferItemTo(to, item); // Двигаем штучки
                                }
                                catch { } // "ловить" поменбше
                            }
                            break; // У меня есть какое-то странное неприятное предчувствие насчет этого
                        }
                    }
                }
                catch {     } // "ловить" поболбше
            }
        }
    }
}
