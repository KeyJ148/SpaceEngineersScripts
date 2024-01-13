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
        public class AssemblersMonitor : BasicMonitor<IMyAssembler, IMyAssembler> {

            public AssemblersMonitor(IDisplay display, List<IMyAssembler> assemblers, string headerText) :
                base(display, headerText, assemblers.ToDictionary(assembler => assembler))
            { }
            protected override string GetName(KeyValuePair<IMyAssembler, IMyAssembler> entity)
            {
                return entity.Key.CustomName;
            }

            protected override string GetInfo(KeyValuePair<IMyAssembler, IMyAssembler> entity)
            {
                IMyAssembler assembler = entity.Key;
                bool work = assembler.IsProducing;
                bool coop = assembler.CooperativeMode;
                bool repeat = assembler.Repeating;

                List<MyProductionItem> productionQueue = new List<MyProductionItem>();
                assembler.GetQueue(productionQueue);
                bool queueEmpty = productionQueue.Count == 0;
                string queue = !queueEmpty ? productionQueue[0].BlueprintId.SubtypeName : "";

                return " " +
                    ((work) ? "W" : " ") +
                    ((coop) ? "C" : " ") +
                    ((repeat) ? "R" : " ") +
                    ((!queueEmpty) ? " - " + queue : "");
            }
        }
    }
}
