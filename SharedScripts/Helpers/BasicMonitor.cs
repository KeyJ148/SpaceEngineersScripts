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
        public abstract class BasicMonitor<K, V> : IMonitor
        {
            protected readonly Display display;
            protected readonly string headerText;
            protected readonly Dictionary<K, V> monitoringEntities;
            protected readonly int maxNameLength;

            public BasicMonitor(Display display, string headerText, Dictionary<K, V> monitoringEntities)
            {
                this.display = display;
                this.headerText = headerText;
                this.monitoringEntities = monitoringEntities;
                maxNameLength = monitoringEntities.ToList().Select(GetName).Select(name => name.Length).Max();
            }

            public virtual void Update()
            { }

            public void Render()
            {
                display.Clear();
                if (headerText != null)
                {
                    display.PrintMiddle(headerText);
                }

                foreach (var entity in monitoringEntities)
                {
                    display.Println(GetOneInfoLine(entity));
                }
            }

            protected abstract string GetName(KeyValuePair<K, V> entity);

            protected string GetOneInfoLine(KeyValuePair<K, V> entity)
            {
                string name = GetName(entity);
                int countSpaceAfterName = maxNameLength - name.Length;
                return name + new string(' ', countSpaceAfterName) + GetInfo(entity);
            }

            protected abstract string GetInfo(KeyValuePair<K, V> entity);
        }
    }
}
