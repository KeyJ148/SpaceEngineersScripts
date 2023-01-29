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
        public abstract class ProgressbarMonitor<T> : GroupsEntitiesMonitor<T> where T : IMyEntity
        {
            protected readonly ProgressbarSettings progressbarSettings;

            public ProgressbarMonitor(IDisplay display, string headerText, ProgressbarSettings progressbarSettings,
                Dictionary<string, List<T>> groupEntityByName) :
                base(display, headerText, groupEntityByName)
            {
                this.progressbarSettings = new ProgressbarSettings(
                    progressBarEmpty: progressbarSettings.ProgressBarEmpty,
                    progressbarFull: progressbarSettings.ProgressbarFull,
                    progressBar100percent: progressbarSettings.ProgressBar100percent,
                    length: display.GetLength() - maxNameLength);
            }

            protected override string GetInfoAboutEntitiesList(List<T> entities)
            {
                long currentSum = entities.Select(entity => GetCurrentValue(entity)).Sum();
                long maxSum = entities.Select(entity => GetMaxValue(entity)).Sum();
                return Utils.GetProgressBar(currentSum, maxSum, progressbarSettings);
            }

            protected abstract long GetCurrentValue(T entity);
            protected abstract long GetMaxValue(T entity);
        }
    }
}
