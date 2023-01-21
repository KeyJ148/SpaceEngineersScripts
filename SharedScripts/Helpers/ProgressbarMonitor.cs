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
        public abstract class ProgressbarMonitor<T> : BasicMonitor<T> where T : IMyEntity
        {
            public ProgressbarMonitor(Display display, string headerText, ProgressbarSettings progressbarSettings,
                Dictionary<string, List<T>> groupEntityByName) :
                base(display, headerText, progressbarSettings, groupEntityByName)
            { }

            
            protected override string GetInfo(List<T> entities)
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
