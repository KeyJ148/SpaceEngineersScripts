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
        /// Вспомогательный класс для GridMonitor. Позволяет считывать данные из грида.
        /// </summary>
        public class GridReader
        {
            public EnergyGrid Energy { get; private set; }
            public StorageGrid Storage { get; private set; }

            public IMyCubeGrid CubeGrid { get; private set; }
            public IMyGridTerminalSystem GridTerminal { get; private set; }


            public GridReader(IMyGridTerminalSystem gridTerminal, IMyCubeGrid cubeGrid)
            {
                GridTerminal = gridTerminal;
                CubeGrid = cubeGrid;
            }


            internal void RescanGrid()
            {

            }

            public class EnergyGrid : Grid
            {
                public override List<IMyCubeBlock> GetBlocks()
                {
                    var list = new List<IMyCubeBlock>();

                    return list;
                }

                public override void RescanGrid()
                {

                }
            }

            public class StorageGrid : Grid
            {
                public override List<IMyCubeBlock> GetBlocks()
                {
                    var list = new List<IMyCubeBlock>();

                    return list;
                }

                public override void RescanGrid()
                {

                }
            }


            public abstract class Grid
            {
                public IMyCubeGrid CubeGrid { get; private set; }
                public IMyGridTerminalSystem GridTerminal { get; private set; }


                /// <summary>
                /// Ебать я аж охуел от этой фабрики, ебучий ",new()", если бы не ChatGPT хуй проссал бы как реализовать это
                /// </summary>
                /// <typeparam name="T"></typeparam>
                /// <param name="gridTerminal"></param>
                /// <param name="cubeGrid"></param>
                /// <returns></returns>
                public static T MakeGrid<T>(IMyGridTerminalSystem gridTerminal, IMyCubeGrid cubeGrid) where T : Grid, new()
                {
                    T result = new T();
                    result.CubeGrid = cubeGrid;
                    result.GridTerminal = gridTerminal;

                    return new T();
                }

                public Grid() { }

                public Grid(IMyGridTerminalSystem gridTerminal, IMyCubeGrid cubeGrid)
                {
                    GridTerminal = gridTerminal;
                    CubeGrid = cubeGrid;
                }

                public abstract void RescanGrid();
                public abstract List<IMyCubeBlock> GetBlocks();
            }
        }
    }
}
