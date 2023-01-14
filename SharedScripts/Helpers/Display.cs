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

			private readonly IMyTextPanel TextPanel;
			private readonly int Length;

			public Display(IMyTextPanel textPanel, int length)
			{
				this.TextPanel = textPanel;
				this.Length = length;
			}

			public Display(IMyTextPanel textPanel) : this(textPanel, 35) { }

			public void Print(string s) { TextPanel.WritePublicText(s, true); }
			public void Print(int i) { Print(i.ToString()); }
			public void Print(long l) { Print(l.ToString()); }
			public void Print(float f) { Print(f.ToString()); }
			public void Print(double d) { Print(d.ToString()); }
			public void Print(VRage.MyFixedPoint fp) { Print(fp.ToString()); }

			public void Println(string s) { Print(s + "\n"); }
			public void Println(int i) { Println(i.ToString()); }
			public void Println(long l) { Println(l.ToString()); }
			public void Println(float f) { Println(f.ToString()); }
			public void Println(double d) { Println(d.ToString()); }
			public void Println(VRage.MyFixedPoint fp) { Println(fp.ToString()); }

			public void PrintMiddle(string s)
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(' ', (Length - s.Length) / 2);
				Println(sb.ToString() + s);
			}
			public void PrintMiddle(int i) { PrintMiddle(i.ToString()); }
			public void PrintMiddle(long l) { PrintMiddle(l.ToString()); }
			public void PrintMiddle(float f) { PrintMiddle(f.ToString()); }
			public void PrintMiddle(double d) { PrintMiddle(d.ToString()); }
			public void PrintMiddle(VRage.MyFixedPoint fp) { PrintMiddle(fp.ToString()); }

			public void Clear() { TextPanel.WritePublicText(""); }
		}
	}
}
