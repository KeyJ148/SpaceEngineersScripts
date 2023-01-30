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
        /// Статический класс планировщика. Позволяет привязать задачи к промежуткам, отличным от 1, 10 и 100 игровых тактов.
        /// </summary>
        static class Scheduler
        {
            public static bool DEBUG { get; set; } = true;
            private static List<SchedulerTask> _taskList = new List<SchedulerTask>();
            private static ulong _tick = 0;
            private static bool _running = false;
            private static int _lastId = 0;
            private static Action<string> LogAction { get; set; }

            private static void Log(object obj)
            {
                if (LogAction != null)
                    LogAction(obj.ToString());
            }
            private static void Debug(object obj)
            {
                if (DEBUG)
                    Log(obj);
            }

            public static void UseLogAction(Action<string> action)
            {
                LogAction = action;
            }

            /// <summary>
            /// Создает копию списка задач планировщика, содержащий ссылки на задачи. Может быть использован для управления 
            /// задачами (поиск, остановка, возобновление, отмена и т.д.)
            /// </summary>
            /// <returns>Копия списка задач</returns>
            public static IEnumerable<SchedulerTask> GetTaskList()
            {
                foreach (var task in _taskList)
                {
                    yield return task;
                }
            }

            public static ulong GetCurrentTick()
            {
                return _tick;
            }

            /// <summary>
            /// Создает задачу, выполняемую каждые несколько тактов
            /// </summary>
            /// <param name="action">Выполняемый метод</param>
            /// <param name="interval">Интервал в тиках</param>
            /// <param name="delay">(не обязательно) Задержка первого выполнения. 0 по умолчанию</param>
            /// <param name="executionTimes">(не обязательно) Количество повторений задачи. 1 по умолчанию</param>
            /// <param name="doInfinitely">(не обязательно) Если true, то будет повторять задачу до ручной отмены. True по умолчанию</param>
            /// <returns>Ссылка на созданную задачу, для ручного управления</returns>
            public static SchedulerTask ExecuteEveryNTicks(Action action, int interval, int delay = 0,
                int executionTimes = 1, bool doInfinitely = true)
            {
                Log("Добавление периодической задачи...");
                return AddTask(action, _tick + (ulong)delay, interval, executionTimes, doInfinitely);
            }


            /// <summary>
            /// Однократное отложенное выполнение задачи
            /// </summary>
            /// <param name="action">Выполняемый метод</param>
            /// <param name="targetTick"></param>
            /// <returns></returns>
            public static SchedulerTask ExecuteAt(Action action, ulong targetTick)
            {
                Log("Добавление отложенной задачи...");
                return AddTask(action, targetTick);
            }


            /// <summary>
            /// Общий метод создания задачи. Возвращает ссылку на задачу, которую можно включить, выключить, отменить 
            /// или назначить теги
            /// </summary>
            /// <param name="action">Выполняемый метод</param>
            /// <param name="nextExecution">Номер такта, на котором будет выполнена задача</param>
            /// <param name="interval">Количество тиков между выполнениями</param>
            /// <param name="executionsRemaining">Количество оставшихся выполнений</param>
            /// <param name="doInfinitely">Выполнять ли задачу бесконечно (игнорирует executionsRemaining)</param>
            /// <returns></returns>
            public static SchedulerTask AddTask(Action action, ulong nextExecution, int interval = 0, int executionsRemaining = 1, bool doInfinitely = false)
            {
                var task = new SchedulerTask(_lastId++, action, nextExecution, interval, executionsRemaining, doInfinitely);
                _taskList.Add(task);
                Log($"Добавлена задача #{task.Id}");
                return task;
            }


            /// <summary>
            /// Запускает планировщик с последнего такта.
            /// </summary>
            public static void Start()
            {
                Log("Планировщик запущен");
                _running = true;
            }


            /// <summary>
            /// Ставит планировщик на паузу.
            /// </summary>
            public static void Stop()
            {
                Log("Планировщик приостановлен");
                _running = false;
            }


            /// <summary>
            /// Эмулирует такт планировщика. Должен вызываться в методе Main.
            /// </summary>
            public static void Tick()
            {
                if (!_running)
                    return;

                Debug($"Такт #{_tick}");
                foreach (var task in new List<SchedulerTask>(_taskList))
                {
                    TryExecute(task);
                }

                _tick++;
            }


            /// <summary>
            /// Удаляет задачу из списка, выполняет её и добавляет эту же задачу в конец списка, если она должна быть выполнена повторно
            /// </summary>
            /// <param name="task"></param>
            private static void TryExecute(SchedulerTask task)
            {
                Debug($"Попытка выполнить задачу #{task.Id}");
                

                if (task.MarkedForCancellation)
                    return;

                if (task.NextExecutionAt <= _tick)
                {
                    try
                    {
                        _taskList.Remove(task);
                    }
                    catch { }
                    task.Execute();
                    if (task.DoInfinitely ||task.ExecutionsRemaining > 0)
                    {
                        task.NextExecutionAt = _tick + (ulong)task.Interval;
                        _taskList.Add(task);
                    }
                }
            }





            /// <summary>
            /// Класс задачи для планировщика
            /// </summary>
            public class SchedulerTask
            {
                public List<string> Tags { get; set; } = new List<string>(); // Список тегов для поиска и фильтрации

                public int Id { get; private set; } // Уникальный Id задачи
                public int ExecutionsRemaining { get; set; } // Оставшееся количество выполнений. Игнорируется, если DoInfinitely = true
                public int Interval { get; set; }
                public ulong NextExecutionAt { get; set; }

                public bool DoInfinitely { get; set; }
                public bool Active { get; private set; } = true; // Выполнять ли задачу при вызове. Управляется методами Activate() и Deactivate()
                public bool MarkedForCancellation { get; private set; } = false; // Если true, то задача будет удалена во время следующего такта

                private Action _action;



                public SchedulerTask(int id, Action action, ulong nextExecution, int interval = 0, int executionsRemaining = 1, bool doInfinitely = false)
                {
                    Id = id;
                    ExecutionsRemaining = executionsRemaining;
                    _action = action;
                    NextExecutionAt = nextExecution;
                    Interval = interval;
                    DoInfinitely = doInfinitely;
                }

                /// <summary>
                /// Добавляет тег задаче для поиска и фильтрации в будущем.
                /// </summary>
                /// <param name="tag"></param>
                public SchedulerTask AddTag(string tag)
                {
                    Tags.Add(tag);
                    return this;
                }

                /// <summary>
                /// Добавляет теги задаче для поиска и фильтрации в будущем
                /// </summary>
                /// <param name="tags"></param>
                public void AddTags(params string[] tags)
                {
                    Tags.AddArray(tags);
                }

                /// <summary>
                /// Выполняет задачу и уменьшает счетчик оставшихся повторений, если задача активна
                /// </summary>
                public void Execute()
                {
                    if (!Active)
                        return;

                    Debug($"Выполняется задача #{Id}");
                    try
                    {
                        _action();
                    }
                    catch { }
                    if(!DoInfinitely)
                        ExecutionsRemaining--;
                }


                /// <summary>
                /// Задача будет удалена из планировщика во время следующего такта.
                /// </summary>
                public void Cancel()
                {
                    MarkedForCancellation = true;
                }

                /// <summary>
                /// Включает выполнение задачи планировщиком.
                /// </summary>
                public void Activate()
                {
                    Active = true;
                }

                /// <summary>
                /// Задача не будет выполняться до тех пор, пока не будет включена методом Activate().
                /// </summary>
                public void Deactivate()
                {
                    Active = false;
                }
            }
        }
    }
}
