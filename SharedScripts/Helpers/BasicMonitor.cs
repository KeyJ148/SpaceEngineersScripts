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
        public abstract class BasicMonitor<T> : IMonitor where T : IMyEntity
        {
            protected readonly Display display;
            protected readonly string headerText;
            protected readonly ProgressbarSettings progressbarSettings;
            protected readonly Dictionary<string, List<T>> groupEntityByName;
            protected readonly int maxNameLength;

            public BasicMonitor(Display display, string headerText, ProgressbarSettings progressbarSettings,
                Dictionary<string, List<T>> groupEntityByName)
            {
                this.display = display;
                this.headerText = headerText;
                this.groupEntityByName = groupEntityByName;
                maxNameLength = groupEntityByName.Keys.Select(name => name.Length).Max();
                this.progressbarSettings = new ProgressbarSettings(
                    progressBarEmpty: progressbarSettings.ProgressBarEmpty,
                    progressbarFull: progressbarSettings.ProgressbarFull,
                    progressBar100percent: progressbarSettings.ProgressBar100percent,
                    lenght: display.Length - maxNameLength);
            }

            public void Render()
            {
                display.Clear();
                if (headerText != null)
                {
                    display.PrintMiddle(headerText);
                }

                foreach (var entry in groupEntityByName)
                {
                    display.Println(GetOneInfoLine(entry.Key, entry.Value));
                }
            }

            protected string GetOneInfoLine(string name, List<T> entities)
            {
                int countSpaceAfterName = maxNameLength - name.Length;
                return name + new string(' ', countSpaceAfterName) + GetInfo(entities);
            }

            protected abstract string GetInfo(List<T> entities);
        }
    }
}
