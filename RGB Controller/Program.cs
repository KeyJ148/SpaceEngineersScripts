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
    /// Контроллер освещения в сети. Делает переливающуюся RGB подсветку.
    /// </summary>
    partial class Program : MyGridProgram
    { 
        // Конфиг
        private const bool UseGroup = true; // Ессли true, то будет искать лампы в группе TargetGroupName
        private const string TargetGroupName = "[GTW] Лампы";
        private const int AngleStep = 5; // Насколько сильно изменяется значение цвета за оди н такт. Значение цвета меняется в промежутке от 0 до 360
        private const int TickInterval = 5; // Сколько игровых тактов проходит между тактами скрипта.
        private const double Saturation = 0.1; // Насыщенность цвета
        private const double Value = 1; // Затенение цвета

        // Скрипт
        private List<IMyLightingBlock> lightGroup = new List<IMyLightingBlock>();
        private int ang = 0;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1; // Частота запуска скрипта

            Scheduler.UseLogAction(Echo); // Передача ссылки на метод Echo в планировщик (для отладки)

            // Задачи планировщика
            Scheduler.ExecuteEveryNTicks(DoRgbControl, TickInterval); // Изменение цвета лампы
            Scheduler.ExecuteEveryNTicks(Scan, 200); // Обновление списка ламп каждые 200 тактов

            // Запуск планировщика
            Scheduler.Start();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // Эмуляция такта планировщика
            Scheduler.Tick();
        }

        /// <summary>
        /// Сканирует систему на наличие ламп. Сканирование системы - ресурсоемкий процесс, поэтому в планировщике 
        /// он запускается редко.
        /// </summary>
        private void Scan()
        {
            var grid = GridTerminalSystem;
            lightGroup.Clear();

            if (UseGroup)
            {
                grid.GetBlockGroupWithName(TargetGroupName).GetBlocksOfType(lightGroup);
            }
            else
            {
                #pragma warning disable CS0162 // Достижимость кода определяется настройками в начале скрипта
                grid.GetBlocksOfType(lightGroup);
                #pragma warning restore CS0162 // Обнаружен недостижимый код
            }
        }

        /// <summary>
        /// Метод управления цветом ламп
        /// </summary>
        private void DoRgbControl()
        {
            // Создаем новый цвет
            var lampColor = ColorUtils.HsvToRgb(ang, Saturation, Value);
            // и применяем его ко всем лампам
            foreach (var light in lightGroup)
            {
                light.Color = lampColor;
            }

            // Изменение угла, определяющего цвет в модели HSV
            ang += AngleStep;
            ang %= 360;
        }
    }
}
