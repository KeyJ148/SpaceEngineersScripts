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
        public abstract class GroupsEntitiesMonitor<T> : BasicMonitor<string, List<T>> where T : IMyEntity
        {

            public GroupsEntitiesMonitor(Display display, string headerText, Dictionary<string, List<T>> groupEntityByName) :
                base(display, headerText, groupEntityByName)
            { }

            protected override string GetName(KeyValuePair<string, List<T>> groupEntityByName)
            {
                return groupEntityByName.Key;
            }

            protected override string GetInfo(KeyValuePair<string, List<T>> groupEntityByName)
            {
                return GetInfoAboutEntitiesList(groupEntityByName.Value);
            }

            protected abstract string GetInfoAboutEntitiesList(List<T> entities);
        }
    }
}
