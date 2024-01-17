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
		public class DisplayGroup : IDisplay
		{
			private readonly int length;
			private readonly List<IMyTextPanel> textPanels;
			private readonly int padding;
			private readonly int symbolsInOneDisplay;

			private int printedSymbolsInCurrentLine = 0;

			public DisplayGroup(List<IMyTextPanel> textPanels, int length, float fontSize, Color fontColor, Color backgroundColor,
				int padding)
			{
				this.length = length;
				this.textPanels = textPanels;
				this.padding = padding;
				symbolsInOneDisplay = length / textPanels.Count;

				foreach (var textPanel in textPanels)
				{
					textPanel.ContentType = ContentType.TEXT_AND_IMAGE;
					textPanel.Font = "Monospace";
					textPanel.FontColor = fontColor;
					textPanel.FontSize = fontSize;
					textPanel.BackgroundColor = backgroundColor;
					textPanel.TextPadding = 0;
				}
				Clear();
			}

			public void Print(object o) 
			{
				o.ToString().Split('\n').Where(s => s.Length > 0).ToList().ForEach(s =>
				{
					PrintInCurrentLine(s);
					NextLine();
				});
			}

			public void Println(object o) 
			{ 
				Print(o.ToString() + "\n"); 
			}

			public void PrintMiddle(object o)
			{
				StringBuilder sb = new StringBuilder();
				int prefixSpaces = Math.Max((length - o.ToString().Length) / 2, 0);
				sb.Append(' ', prefixSpaces);
				Println(sb.ToString() + o.ToString());
			}

			public void Clear()
			{
				foreach (var textPanel in textPanels)
				{
					textPanel.WriteText("");
				}
				PrintPadding();
			}

			public int GetLength()
			{
				return length;
			}

			private void PrintInCurrentLine(string s)
            {
				IMyTextPanel currentPanel;
				string currentPanelText;
				do
				{
					int currentPanelIndex = printedSymbolsInCurrentLine / symbolsInOneDisplay < textPanels.Count ?
						printedSymbolsInCurrentLine / symbolsInOneDisplay : textPanels.Count - 1;
					currentPanel = textPanels[currentPanelIndex];
					int symbolsToEndCurrentPanel = symbolsInOneDisplay - (printedSymbolsInCurrentLine % symbolsInOneDisplay);
					currentPanelText = s.Length > symbolsToEndCurrentPanel ? s.Substring(0, symbolsToEndCurrentPanel) : s;

					currentPanel.WriteText(currentPanelText, true);
					printedSymbolsInCurrentLine += currentPanelText.Length;
					s = s.Substring(currentPanelText.Length);
				} while (s.Length > 0 && currentPanel != textPanels.Last());
				textPanels.Last().WriteText(s, true);
			}

			private void NextLine()
            {
				foreach (var textPanel in textPanels)
				{
					textPanel.WriteText("\n", true);
				}
				PrintPadding();
				printedSymbolsInCurrentLine = 0;
            }

			private void PrintPadding()
            {
				textPanels[0].WriteText(new string(' ', padding), true);
			}
		}
	}
}
