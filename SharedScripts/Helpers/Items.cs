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

		public static class Items
		{

			public static class Ores
			{
				public static readonly Ore STONE = new Ore("MyObjectBuilder_Ore/Stone", "Камень", 46800);
				public static readonly Ore IRON = new Ore("MyObjectBuilder_Ore/Iron", "Железо", 93600);
				public static readonly Ore SILICON = new Ore("MyObjectBuilder_Ore/Silicon", "Кремний", 7800);
				public static readonly Ore NICKEL = new Ore("MyObjectBuilder_Ore/Nickel", "Никель", 2340);
				public static readonly Ore COBALT = new Ore("MyObjectBuilder_Ore/Cobalt", "Кобальт", 1170);
				public static readonly Ore MAGNESIUM = new Ore("MyObjectBuilder_Ore/Magnesium", "Магний", 4680);
				public static readonly Ore SILVER = new Ore("MyObjectBuilder_Ore/Silver", "Серебро", 4680);
				public static readonly Ore GOLD = new Ore("MyObjectBuilder_Ore/Gold", "Золото", 11700);
				public static readonly Ore PLATINUM = new Ore("MyObjectBuilder_Ore/Platinum", "Платина", 1170);
				public static readonly Ore URANIUM = new Ore("MyObjectBuilder_Ore/Uranium", "Уран", 1170);
				public static readonly Item ICE = new Item("MyObjectBuilder_Ore/Ice", "Лёд");

			}

			public static class Ingots
			{
				public static readonly Item STONE = new Item("MyObjectBuilder_Ingot/Stone", "Гравий");
				public static readonly Item IRON = new Item("MyObjectBuilder_Ingot/Iron", "Железо");
				public static readonly Item SILICON = new Item("MyObjectBuilder_Ingot/Silicon", "Кремний");
				public static readonly Item NICKEL = new Item("MyObjectBuilder_Ingot/Nickel", "Никель");
				public static readonly Item COBALT = new Item("MyObjectBuilder_Ingot/Cobalt", "Кобальт");
				public static readonly Item MAGNESIUM = new Item("MyObjectBuilder_Ingot/Magnesium", "Магний");
				public static readonly Item SILVER = new Item("MyObjectBuilder_Ingot/Silver", "Серебро");
				public static readonly Item GOLD = new Item("MyObjectBuilder_Ingot/Gold", "Золото");
				public static readonly Item PLATINUM = new Item("MyObjectBuilder_Ingot/Platinum", "Платина");
				public static readonly Item URANIUM = new Item("MyObjectBuilder_Ingot/Uranium", "Уран");
			}

			public static class Components
			{
				public static readonly Item STEEL_PLATE = new Item("MyObjectBuilder_Component/SteelPlate", "Сталь");
				public static readonly Item INTEROR_PLATE = new Item("MyObjectBuilder_Component/InteriorPlate", "Внутр. пл.");
				public static readonly Item CONSTRUCTION = new Item("MyObjectBuilder_Component/Construction", "Стройка");
				public static readonly Item COMPUTER = new Item("MyObjectBuilder_Component/Computer", "Компьютер");
				public static readonly Item MOTOR = new Item("MyObjectBuilder_Component/Motor", "Мотор");
				public static readonly Item GIRDER = new Item("MyObjectBuilder_Component/Girder", "Балка");
				public static readonly Item SMALL_TUBE = new Item("MyObjectBuilder_Component/SmallTube", "Малая т.");
				public static readonly Item LARGE_TUBE = new Item("MyObjectBuilder_Component/LargeTube", "Большая т.");
				public static readonly Item METAL_GRID = new Item("MyObjectBuilder_Component/MetalGrid", "Решётка");
				public static readonly Item DISPLAY = new Item("MyObjectBuilder_Component/Display", "Экран");
				public static readonly Item BULLETPROOF_GLASS = new Item("MyObjectBuilder_Component/BulletproofGlass", "Стекло");
				public static readonly Item POWER_CELL = new Item("MyObjectBuilder_Component/PowerCell", "Батарея");
				public static readonly Item RADIO_COMMUNICATION = new Item("MyObjectBuilder_Component/RadioCommunication", "Радио");
				public static readonly Item MEDICAL = new Item("MyObjectBuilder_Component/Medical", "Медицина");
				public static readonly Item REACTOR = new Item("MyObjectBuilder_Component/Reactor", "Реактор");
				public static readonly Item THRUST = new Item("MyObjectBuilder_Component/Thrust", "Ускоритель");
				public static readonly Item DETECTOR = new Item("MyObjectBuilder_Component/Detector", "Детектор");
				public static readonly Item GRAVITY_GENERATOR = new Item("MyObjectBuilder_Component/GravityGenerator", "Грав. ген.");
				public static readonly Item EXPLOSIVES = new Item("MyObjectBuilder_Component/Explosives", "Взрывчатка");
				public static readonly Item SOLAR_CELL = new Item("MyObjectBuilder_Component/SolarCell", "Солн. пан.");
			}

			public static class Instruments
			{
				public static readonly Item WELDER_T0 = new Item("MyObjectBuilder_PhysicalGunObject/WelderItem", "Сварщик Т0");
				public static readonly Item GRINDER_T0 = new Item("MyObjectBuilder_PhysicalGunObject/AngelGrinderItem", "Резак Т0");
				public static readonly Item DRILL_T0 = new Item("MyObjectBuilder_PhysicalGunObject/HandDrillItem", "Бур Т0");
				public static readonly Item WELDER_T1 = new Item("MyObjectBuilder_PhysicalGunObject/Welder2Item", "Сварщик Т1");
				public static readonly Item GRINDER_T1 = new Item("MyObjectBuilder_PhysicalGunObject/AngelGrinder2Item", "Резак Т1");
				public static readonly Item DRILL_T1 = new Item("MyObjectBuilder_PhysicalGunObject/HandDrill2Item", "Бур Т1");
				public static readonly Item WELDER_T2 = new Item("MyObjectBuilder_PhysicalGunObject/Welder3Item", "Сварщик Т2");
				public static readonly Item GRINDER_T2 = new Item("MyObjectBuilder_PhysicalGunObject/AngelGrinder3Item", "Резак Т2");
				public static readonly Item DRILL_T2 = new Item("MyObjectBuilder_PhysicalGunObject/HandDrill3Item", "Бур Т2");
				public static readonly Item WELDER_T3 = new Item("MyObjectBuilder_PhysicalGunObject/Welder4Item", "Сварщик Т3");
				public static readonly Item GRINDER_T3 = new Item("MyObjectBuilder_PhysicalGunObject/AngelGrinder4Item", "Резак Т3");
				public static readonly Item DRILL_T3 = new Item("MyObjectBuilder_PhysicalGunObject/HandDrill4Item", "Бур Т3");
				public static readonly Item HYDROGEN_BOTTLE = new Item("MyObjectBuilder_GasContainerObject/HydrogenBottle", "Баллон H2");
				public static readonly Item OXYGEN_BOTTLE = new Item("MyObjectBuilder_OxygenContainerObject/OxygenBottle", "Баллон O2");
			}

			public static class Weapons
			{

			}

			public static class Ammo
			{

			}

			public static readonly List<Ore> ORES = new List<Ore> {
				Ores.STONE, Ores.IRON, Ores.SILICON, Ores.NICKEL, Ores.COBALT, Ores.MAGNESIUM, Ores.SILVER, Ores.GOLD, Ores.PLATINUM,
				Ores.URANIUM
			};

			public static readonly List<Item> INGOTS = new List<Item> {
				Ingots.STONE, Ingots.IRON, Ingots.SILICON, Ingots.NICKEL, Ingots.COBALT, Ingots.MAGNESIUM, Ingots.SILVER, Ingots.GOLD,
				Ingots.PLATINUM, Ingots.URANIUM
			};

			public static readonly List<Item> COMPONENTS = new List<Item> {
				Components.STEEL_PLATE, Components.INTEROR_PLATE, Components.CONSTRUCTION, Components.COMPUTER, Components.MOTOR,
				Components.GIRDER, Components.SMALL_TUBE, Components.LARGE_TUBE, Components.METAL_GRID, Components.DISPLAY,
				Components.BULLETPROOF_GLASS, Components.POWER_CELL, Components.RADIO_COMMUNICATION, Components.MEDICAL,
				Components.REACTOR, Components.THRUST, Components.DETECTOR, Components.GRAVITY_GENERATOR, Components.EXPLOSIVES,
				Components.SOLAR_CELL
			};

			public static readonly List<Item> INSTRUMENTS = new List<Item> {
				Instruments.WELDER_T0, Instruments.GRINDER_T0, Instruments.DRILL_T0,
				Instruments.WELDER_T1, Instruments.GRINDER_T1, Instruments.DRILL_T1,
				Instruments.WELDER_T2, Instruments.GRINDER_T2, Instruments.DRILL_T2,
				Instruments.WELDER_T3, Instruments.GRINDER_T3, Instruments.DRILL_T3
			};

			public static readonly List<Item> OTHER = new List<Item> {
				Ores.ICE, Instruments.HYDROGEN_BOTTLE, Instruments.OXYGEN_BOTTLE
			};

			public static readonly List<Item> ALL = ORES.Concat(INGOTS).Concat(COMPONENTS).Concat(INSTRUMENTS).Concat(OTHER).ToList();
		}

		public class Item
		{

			public readonly MyDefinitionId Id;
			public readonly string Name;

			protected Item(string id, string name) {
				Id = MyDefinitionId.Parse(id);
				Name = name;
			}
		}

		public class Ore : Item
		{

			public readonly int RefineSpeed; //kg/hour

			protected Ore(string id, string name, int refineSpeed) : base(id, name) {
				RefineSpeed = refineSpeed;
			}

		}
	}
}
