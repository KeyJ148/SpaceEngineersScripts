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
        private int _tier = 1; // степень 10, на которую будет умножено требуемое количество предметов

        // Список предметов для дозаказа
        private readonly Dictionary<Item, long> _targetAmount = new Dictionary<Item, long>
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
            { Items.Components.EXPLOSIVES, 100 }
        };

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;

            Scheduler.ExecuteEveryNTicks(ScanGrid, 200);
            Scheduler.ExecuteEveryNTicks(CheckStorage, 50);

            Scheduler.Start();
        }

        private void StartProducing(Item component, int amount)
        {

        }

        private void ScanGrid()
        {

        }

        private void CheckStorage()
        {

        }

        public void Main(string argument, UpdateType updateSource)
        {
            Scheduler.Tick();
        }
    }
}
