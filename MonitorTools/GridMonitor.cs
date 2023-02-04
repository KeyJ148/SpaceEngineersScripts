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
        /// <summary>
        /// Класс монитора для указанного грида. В конструктор нужно передать ссылку на грид.
        /// </summary>
        public class GridMonitor
        {
            private IMyGridTerminalSystem _gridTerminal;
            private IMyCubeGrid _cubeGrid;

            private GridReader reader;

            public GridMonitor(IMyGridTerminalSystem gridTerminalSystem, IMyCubeGrid cubeGrid)
            {
                _gridTerminal = gridTerminalSystem;
                _cubeGrid = cubeGrid;

                reader = new GridReader(_gridTerminal, _cubeGrid);
            }

            /// <summary>
            /// Повторяет сканирование конструкции с поиском и кэшированием объектов
            /// </summary>
            public void RescanGrid()
            {
                reader.RescanGrid();
            }
        }
    }
}
