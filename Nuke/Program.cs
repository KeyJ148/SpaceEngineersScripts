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
	partial class Program : MyGridProgram
	{
		private List<IMyReactor> reactors = new List<IMyReactor>();
		private List<IMyBatteryBlock> batteries = new List<IMyBatteryBlock>();

		private int cyclesCounter = 0;
		private const int
			maxCycles = 10; // Used to rescan system for energy blocks

		private const double
			minChargePercentage = 0.6, // Start reactors below this percentage
			maxChargePercentage = 0.8; // Stop reactors above this percentage



		public Program()
		{
			Runtime.UpdateFrequency = UpdateFrequency.Update100;
			Reset();
		}


		private void Reset()
		{
			// Reset cycles counter
			cyclesCounter = 0;

			// Clear energy system cache
			reactors = new List<IMyReactor>();
			batteries = new List<IMyBatteryBlock>();

			// Scan energy system
			GridTerminalSystem.GetBlocksOfType(reactors);
			GridTerminalSystem.GetBlocksOfType(batteries);
		}


		public void Main(string argument, UpdateType updateSource)
		{
			cyclesCounter++;
			if (cyclesCounter >= maxCycles)
				Reset();


			var chargeLevel = GetStoredEnergyPercentage();
			

			if (chargeLevel <= minChargePercentage)
			{
				ToggleReactors(true);
			}
			if (chargeLevel >= maxChargePercentage)
			{
				ToggleReactors(false);
			}
		}


		/// <summary>
		/// Get summary batteries charge
		/// </summary>
		/// <returns>double in range 0.0 - 1.0</returns>
		private double GetStoredEnergyPercentage()
		{
			var stored = 0.0;
			var maxCapacity = 0.0;

			foreach (var battery in batteries)
			{
				stored = battery.CurrentStoredPower;
				maxCapacity = battery.MaxStoredPower;
			}

			return stored/maxCapacity;
		}


		/// <summary>
		/// Enable/Disable reactors
		/// </summary>
		/// <param name="state"></param>
		private void ToggleReactors(bool state)
		{
			foreach (var reactor in reactors)
			{
				reactor.Enabled = state;
			}
		}
	}
}
