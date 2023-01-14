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
    partial class Program : MyGridProgram
    {
        private readonly List<Monitor> Monitors = new List<Monitor>();

        //TODO (перенести в игру или сюда из игры? Это же TestScript, без гита):
        /*
         * Builder для RefinerMonitor
         * EnergyMonitor
         * Выравнивание по пробелам в RefinerMonitor
         * Самому вычислять количество рефайнеров, тоже в SharedScripts
         * Запускать Refiner монитор в двух режимах: только общие заводы или только типизированные
         * В скриптах всё в UFT-8 и LF
         * Кнопка для перезагрзки командных блоков
         * (т.к. надо заново иницировать после новых построек)
         */

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;

            List<IMyEntity> allContainers = new List<IMyEntity>();
            GridTerminalSystem.GetBlocksOfType<IMyEntity>(allContainers);

            Monitors.Add(new RefinersMonitor(
                new Display(GridTerminalSystem.GetBlockWithName("Дисплей - основной") as IMyTextPanel, 70),
                Items.ORES,
                new Dictionary<Ore, int>(),
                3,
                allContainers,
                "ОЧИСТИТЕЛЬНЫЕ ЗАВОДЫ"
            ));
        }

        public void Main(string argument, UpdateType updateSource)
        {
            Monitors.ForEach(monitor => monitor.Render());
        }
    }
}
