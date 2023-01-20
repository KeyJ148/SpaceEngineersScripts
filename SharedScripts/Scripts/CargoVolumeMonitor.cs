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
        public class CargoVolumeMonitor : ProgressbarMonitor<IMyEntity>
        {
            public CargoVolumeMonitor(Display display, string headerText, ProgressbarSettings progressbarSettings,
                   Dictionary<string, List<IMyEntity>> groupEntityByName) :
                   base(display, headerText, progressbarSettings, groupEntityByName)
            { }

            protected override long GetCurrentValue(IMyEntity entity)
            {
                List<IMyInventory> inventories = Utils.GetAllInventory(entity);
                return inventories.Select(inventory => Utils.FromRaw(inventory.CurrentVolume.RawValue) * 1000).Sum();
            }

            protected override long GetMaxValue(IMyEntity entity)
            {
                List<IMyInventory> inventories = Utils.GetAllInventory(entity);
                return inventories.Select(inventory => Utils.FromRaw(inventory.MaxVolume.RawValue) * 1000).Sum();
            }
        }
    }
}
