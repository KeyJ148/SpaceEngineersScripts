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
		public class Display
		{

			public readonly int Length;
			private readonly IMyTextPanel textPanel;

			public Display(IMyTextPanel textPanel, int length)
			{
				Length = length;
				this.textPanel = textPanel;
			}

			public Display(IMyTextPanel textPanel) : this(textPanel, 35) { }

			public void Print(Object o) 
			{
				textPanel.WritePublicText(o.ToString(), true);
			}

			public void Println(Object o) 
			{ 
				Print(o.ToString() + "\n"); 
			}

			public void PrintMiddle(Object o)
			{
				StringBuilder sb = new StringBuilder();
				int prefixSpaces = Math.Max((Length - o.ToString().Length) / 2, 0);
				sb.Append(' ', prefixSpaces);
				Println(sb.ToString() + o.ToString());
			}

			public void Clear()
			{
				textPanel.WritePublicText("");
			}
		}
	}
}
