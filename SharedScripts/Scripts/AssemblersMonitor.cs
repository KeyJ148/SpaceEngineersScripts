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
        public class AssemblersMonitor : IMonitor
        {
            private readonly Display display;
            private readonly List<IMyAssembler> assemblers;
            private readonly int maxNameLength;
            private readonly string headerText;

            public AssemblersMonitor(Display display, List<IMyAssembler> assemblers, string headerText)
            {
                this.display = display;
                this.assemblers = assemblers;
                this.headerText = headerText;
                maxNameLength = assemblers.Select(assembler => assembler.CustomName.Length).Max();
            }

            public void Render()
            {
                display.Clear();
                if (headerText != null)
                {
                    display.PrintMiddle(headerText);
                }

                foreach (IMyAssembler assembler in assemblers)
                {
                    display.Println(GetAssemblerInfoLine(assembler));
                }
            }

            private String GetAssemblerInfoLine(IMyAssembler assembler)
            {
                int countSpaceAfterName = maxNameLength - assembler.CustomName.Length;
                bool work = assembler.IsProducing;
                bool coop = assembler.CooperativeMode;
                bool repeat = assembler.Repeating;

                List<MyProductionItem> productionQueue = new List<MyProductionItem>();
                assembler.GetQueue(productionQueue);
                bool queueEmpty = productionQueue.Count == 0;
                string queue = !queueEmpty ? productionQueue[0].BlueprintId.SubtypeName : "";

                return assembler.CustomName +
                    new string(' ', countSpaceAfterName) +
                    ((work) ? "W" : " ") +
                    ((coop) ? "C" : " ") +
                    ((repeat) ? "R" : " ") +
                    ((!queueEmpty) ? " - " + queue : "");
            }
        }
    }
}
