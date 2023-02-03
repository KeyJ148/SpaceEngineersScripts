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
	/// <summary>
    /// Эта библиотека может быть полезна сама по ебе и в других скриптах
    /// </summary>
    partial class Program
    {
		/// <summary>
		/// Содержит перечисления игровых предметов
		/// </summary>
		public static class Items
		{
			/// <summary>
			/// Содержит перечисление руд
			/// </summary>
			public static class Ores
			{
				private static readonly String PREFIX = "MyObjectBuilder_Ore/";
				public static readonly Ore STONE = new Ore(PREFIX + "Stone", "Камень", 130, 0.0504, Ingots.STONE);
				public static readonly Ore IRON = new Ore(PREFIX + "Iron", "Железо", 26, 0.7, Ingots.IRON);
				public static readonly Ore SILICON = new Ore(PREFIX + "Silicon", "Кремний", 2.167, 0.7, Ingots.SILICON);
				public static readonly Ore NICKEL = new Ore(PREFIX + "Nickel", "Никель", 1.97, 0.4, Ingots.NICKEL);
				public static readonly Ore COBALT = new Ore(PREFIX + "Cobalt", "Кобальт", 0.433, 0.3, Ingots.COBALT);
				public static readonly Ore MAGNESIUM = new Ore(PREFIX + "Magnesium", "Магний", 2.6, 0.007, Ingots.MAGNESIUM);
				public static readonly Ore SILVER = new Ore(PREFIX + "Silver", "Серебро", 1.3, 0.1, Ingots.SILVER);
				public static readonly Ore GOLD = new Ore(PREFIX + "Gold", "Золото", 3.25, 0.01, Ingots.GOLD);
				public static readonly Ore PLATINUM = new Ore(PREFIX + "Platinum", "Платина", 0.433, 0.005, Ingots.PLATINUM);
				public static readonly Ore URANIUM = new Ore(PREFIX + "Uranium", "Уран", 0.325, 0.01, Ingots.URANIUM);
				public static readonly Item ICE = new Item(PREFIX + "Ice", "Лёд");

			}

			/// <summary>
			/// Содержит перечисление ресурсов
			/// </summary>
			public static class Ingots
			{
				private static readonly String PREFIX = "MyObjectBuilder_Ingot/";
				public static readonly Item STONE = new Item(PREFIX + "Stone", "Гравий");
				public static readonly Item IRON = new Item(PREFIX + "Iron", "Железо");
				public static readonly Item SILICON = new Item(PREFIX + "Silicon", "Кремний");
				public static readonly Item NICKEL = new Item(PREFIX + "Nickel", "Никель");
				public static readonly Item COBALT = new Item(PREFIX + "Cobalt", "Кобальт");
				public static readonly Item MAGNESIUM = new Item(PREFIX + "Magnesium", "Магний");
				public static readonly Item SILVER = new Item(PREFIX + "Silver", "Серебро");
				public static readonly Item GOLD = new Item(PREFIX + "Gold", "Золото");
				public static readonly Item PLATINUM = new Item(PREFIX + "Platinum", "Платина");
				public static readonly Item URANIUM = new Item(PREFIX + "Uranium", "Уран");
			}

			/// <summary>
			/// Содержит перечисление строительных компонентов
			/// </summary>
			public static class Components
			{
				private static readonly String PREFIX = "MyObjectBuilder_Component/";
				public static readonly Item STEEL_PLATE = new Item(PREFIX + "SteelPlate", "Сталь");
				public static readonly Item INTEROR_PLATE = new Item(PREFIX + "InteriorPlate", "Внутр. пл.");
				public static readonly Item CONSTRUCTION = new Item(PREFIX + "Construction", "Стройка");
				public static readonly Item COMPUTER = new Item(PREFIX + "Computer", "Компьютер");
				public static readonly Item MOTOR = new Item(PREFIX + "Motor", "Мотор");
				public static readonly Item GIRDER = new Item(PREFIX + "Girder", "Балка");
				public static readonly Item SMALL_TUBE = new Item(PREFIX + "SmallTube", "Малая т.");
				public static readonly Item LARGE_TUBE = new Item(PREFIX + "LargeTube", "Большая т.");
				public static readonly Item METAL_GRID = new Item(PREFIX + "MetalGrid", "Решётка");
				public static readonly Item DISPLAY = new Item(PREFIX + "Display", "Экран");
				public static readonly Item BULLETPROOF_GLASS = new Item(PREFIX + "BulletproofGlass", "Стекло");
				public static readonly Item POWER_CELL = new Item(PREFIX + "PowerCell", "Батарея");
				public static readonly Item RADIO_COMMUNICATION = new Item(PREFIX + "RadioCommunication", "Радио");
				public static readonly Item MEDICAL = new Item(PREFIX + "Medical", "Медицина");
				public static readonly Item REACTOR = new Item(PREFIX + "Reactor", "Реактор");
				public static readonly Item THRUST = new Item(PREFIX + "Thrust", "Ускоритель");
				public static readonly Item DETECTOR = new Item(PREFIX + "Detector", "Детектор");
				public static readonly Item GRAVITY_GENERATOR = new Item(PREFIX + "GravityGenerator", "Грав. ген.");
				public static readonly Item EXPLOSIVES = new Item(PREFIX + "Explosives", "Взрывчатка");
				public static readonly Item SOLAR_CELL = new Item(PREFIX + "SolarCell", "Солн. пан.");
			}


			/// <summary>
			/// Содержит перечисление инструментов (T0-T3 +баллоны)
			/// </summary>
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

			// TODO: Перечисление боеприпасов
			public static class Ammo
			{

			}

			// TODO: Перечисление оружий
			public static class Weapons
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

			public Item(string id, string name)
			{
				Id = MyDefinitionId.Parse(id);
				Name = name;
			}
		}

		public class Ore : Item
		{

			public readonly double RefineSpeed; //kg/sec
			public readonly double RefineEfficiency; //result from ore
			public readonly Item Ingot; //result after refine

			public Ore(string id, string name, double refineSpeed, double refineEfficiency, Item ingot) : base(id, name)
			{
				RefineSpeed = refineSpeed;
				RefineEfficiency = refineEfficiency;
				Ingot = ingot;
			}
		}

		// TODO: режим боли
		public class Weapon : Item
		{
			public readonly Ammo Ammo;
			public readonly double DamagePerShot;
			public readonly double ReloadTime; // sec
			public readonly double FireRate; // shots/sec

			public Weapon(string id, string name, Ammo ammo, double damage, double reloadTime, double fireRate) : base(id, name)
			{
				Ammo = ammo;
				DamagePerShot = damage;
				ReloadTime = reloadTime;
				FireRate = fireRate;
			}

		}

		public class Ammo : Item
		{
			public readonly int MaxAmmo;

			public Ammo(string id, string name, int maxAmmo) : base(id, name)
			{
				MaxAmmo = maxAmmo;
			}
		}
	}
}
