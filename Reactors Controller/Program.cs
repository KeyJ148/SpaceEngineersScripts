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
    /// Контроллер управляет ядерными реакторами для экономии ядерного топлива. 
    /// В зависимости от уровня заряда батарей реаторы включаются и выключаются.
    /// </summary>
    partial class Program : MyGridProgram
    {
        // Конфиг
        private const double
            minChargePercentage = 0.6, // Реакторы запускаются, если заряд батарей ниже этого уровня
            maxChargePercentage = 0.8; // Реакторы останавливаются, если заряд батареи выше этого уровня

        // Скрипт
        private List<IMyReactor> reactors = new List<IMyReactor>();
        private List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100; // Запуск метода Main каждые 100 игровых тактов

            Scheduler.UseLogAction(Echo); // Передача ссылки на метод Echo в планировщик (для отладки)

            // Задачи планировщика
            Scheduler.ExecuteEveryNTicks(Reset, 10); // Пересканирование энергосистемы каждые 10 тактов планировщика (1000 игровых)
            Scheduler.ExecuteEveryNTicks(CheckEnergySystem, 1); // Проверка энергосистемы и запуск/остановка реакторов

            Scheduler.Start(); // Запуск планировщика
        }


        public void Main(string argument, UpdateType updateSource)
        {
            Scheduler.Tick(); // Такт планировщика
        }


        /// <summary>
        /// Пересканирует энергосистему на случай удаления/добавления. Сканирование - ресурсоемкий процесс, поэтому он 
        /// запускается редко.
        /// </summary>
        private void Reset()
        {
            // Очистка кэша энергосистемы
            reactors = new List<IMyReactor>();
            batteries = new List<IMyBatteryBlock>();

            // Сканирование энергосистемы
            GridTerminalSystem.GetBlocksOfType(reactors, reactor => reactor.IsSameConstructAs(Me));
            GridTerminalSystem.GetBlocksOfType(batteries, battery => battery.IsSameConstructAs(Me));
        }


        /// <summary>
        /// Проверка энергосистемы и включение/выключение реакторов, в зависимости от уровня заряда батарей.
        /// </summary>
        private void CheckEnergySystem()
        {
            var chargeLevel = GetStoredEnergyPercentage();


            if (chargeLevel <= minChargePercentage)
            {
                ToggleReactors(true);
            }
            if (chargeLevel >= maxChargePercentage)
            {
                ToggleReactors(false);
            }
        }

        /// <summary>
		/// Получает суммарный заряд батареи
		/// </summary>
		/// <returns>double in range 0.0 - 1.0</returns>
		private double GetStoredEnergyPercentage()
        {
            var stored = 0.0;
            var maxCapacity = 0.0;

            foreach (var battery in batteries)
            {
                stored = battery.CurrentStoredPower;
                maxCapacity = battery.MaxStoredPower;
            }

            return stored / maxCapacity;
        }


        /// <summary>
        /// Включить/отключить реакторы
        /// </summary>
        /// <param name="state"></param>
        private void ToggleReactors(bool state)
        {
            foreach (var reactor in reactors)
            {
                try
                {
                    reactor.Enabled = state;
                }
                catch { }
            }
        }
    }
}
