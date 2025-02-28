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
            /// Содержит перечисление строительных компонентов
            /// </summary>
            public static class Components
            {
                private static readonly String PREFIX = "MyObjectBuilder_Component/";
                public static readonly CraftableItem STEEL_PLATE = new CraftableItem(PREFIX + "SteelPlate", "Сталь")
                    .WithTime(1)
                    .WithIngredients(Ingots.IRON, 21);
                public static readonly CraftableItem INTEROR_PLATE = new CraftableItem(PREFIX + "InteriorPlate", "Внутр. пл.")
                    .WithTime(1)
                    .WithIngredients(Ingots.IRON, 3.5);
                public static readonly CraftableItem CONSTRUCTION = new CraftableItem(PREFIX + "Construction", "Стройка")
                    .WithTime(1.5)
                    .WithIngredients(Ingots.IRON, 10);
                public static readonly CraftableItem COMPUTER = new CraftableItem(PREFIX + "Computer", "Компьютер")
                    .WithTime(1)
                    .WithIngredients(Ingots.IRON, 0.5, Ingots.SILICON, 0.2);
                public static readonly CraftableItem MOTOR = new CraftableItem(PREFIX + "Motor", "Мотор")
                    .WithTime(1)
                    .WithIngredients(Ingots.IRON, 20, Ingots.NICKEL, 5);
                public static readonly CraftableItem GIRDER = new CraftableItem(PREFIX + "Girder", "Балка")
                    .WithTime(1)
                    .WithIngredients(Ingots.IRON, 7);
                public static readonly CraftableItem SMALL_TUBE = new CraftableItem(PREFIX + "SmallTube", "Малая т.")
                    .WithTime(1)
                    .WithIngredients(Ingots.IRON, 5);
                public static readonly CraftableItem LARGE_TUBE = new CraftableItem(PREFIX + "LargeTube", "Большая т.")
                    .WithTime(1)
                    .WithIngredients(Ingots.IRON, 30);
                public static readonly CraftableItem METAL_GRID = new CraftableItem(PREFIX + "MetalGrid", "Решётка")
                    .WithTime(2)
                    .WithIngredients(Ingots.IRON, 12, Ingots.NICKEL, 5, Ingots.COBALT, 3);
                public static readonly CraftableItem DISPLAY = new CraftableItem(PREFIX + "Display", "Экран")
                    .WithTime(1)
                    .WithIngredients(Ingots.IRON, 1, Ingots.SILICON, 5);
                public static readonly CraftableItem BULLETPROOF_GLASS = new CraftableItem(PREFIX + "BulletproofGlass", "Стекло")
                    .WithTime(1)
                    .WithIngredients(Ingots.SILICON, 15);
                public static readonly CraftableItem POWER_CELL = new CraftableItem(PREFIX + "PowerCell", "Батарея")
                    .WithTime(4)
                    .WithIngredients(Ingots.IRON, 10, Ingots.NICKEL, 2, Ingots.SILICON, 1);
                public static readonly CraftableItem RADIO_COMMUNICATION = new CraftableItem(PREFIX + "RadioCommunication", "Радио")
                    .WithTime(1)
                    .WithIngredients(Ingots.IRON, 8, Ingots.SILICON, 1);
                public static readonly CraftableItem MEDICAL = new CraftableItem(PREFIX + "Medical", "Медицина")
                    .WithTime(1)
                    .WithIngredients(Ingots.IRON, 60, Ingots.NICKEL, 70, Ingots.SILVER, 20);
                public static readonly CraftableItem REACTOR = new CraftableItem(PREFIX + "Reactor", "Реактор")
                    .WithTime(1)
                    .WithIngredients(Ingots.IRON, 15, Ingots.SILVER, 5, Ingots.STONE, 20);
                public static readonly CraftableItem THRUST = new CraftableItem(PREFIX + "Thrust", "Ускоритель")
                    .WithTime(1)
                    .WithIngredients(Ingots.IRON, 30, Ingots.COBALT, 10, Ingots.GOLD, 1, Ingots.PLATINUM, 0.4);
                public static readonly CraftableItem DETECTOR = new CraftableItem(PREFIX + "Detector", "Детектор")
                    .WithTime(1)
                    .WithIngredients(Ingots.IRON, 5, Ingots.NICKEL, 15);
                public static readonly CraftableItem GRAVITY_GENERATOR = new CraftableItem(PREFIX + "GravityGenerator", "Грав. ген.")
                    .WithTime(1)
                    .WithIngredients(Ingots.IRON, 600, Ingots.COBALT, 220, Ingots.SILVER, 5, Ingots.GOLD, 10);
                public static readonly CraftableItem EXPLOSIVES = new CraftableItem(PREFIX + "Explosives", "Взрывчатка")
                    .WithTime(10)
                    .WithIngredients(Ingots.MAGNESIUM, 2, Ingots.SILICON, 0.5);
                public static readonly CraftableItem SOLAR_CELL = new CraftableItem(PREFIX + "SolarCell", "Солн. пан.")
                    .WithTime(10)
                    .WithIngredients(Ingots.NICKEL, 10, Ingots.SILICON, 8);
                public static readonly CraftableItem SUPERCONDUCTOR = new CraftableItem(PREFIX + "Superconductor", "Сверхпроводник")
                    .WithTime(8)
                    .WithIngredients(Ingots.IRON, 10, Ingots.GOLD, 2);
            }
            /// <summary>
            /// Содержит перечисление руд
            /// </summary>
            public static class Ores
            {
                private static readonly String PREFIX = "MyObjectBuilder_Ore/";
                public static readonly Ore STONE = new Ore(PREFIX + "Stone", "Камень", 130, 0.0604, Ingots.STONE);
                public static readonly Ore IRON, Fe = IRON = new Ore(PREFIX + "Iron", "Железо", 26, 0.7, Ingots.IRON);
                public static readonly Ore SILICON, Si = SILICON = new Ore(PREFIX + "Silicon", "Кремний", 2.167, 0.7, Ingots.SILICON);
                public static readonly Ore NICKEL, Ni = NICKEL = new Ore(PREFIX + "Nickel", "Никель", 1.97, 0.4, Ingots.NICKEL);
                public static readonly Ore COBALT, Co = COBALT = new Ore(PREFIX + "Cobalt", "Кобальт", 0.433, 0.3, Ingots.COBALT);
                public static readonly Ore MAGNESIUM, Mg = MAGNESIUM = new Ore(PREFIX + "Magnesium", "Магний", 2.6, 0.007, Ingots.MAGNESIUM);
                public static readonly Ore SILVER, Ag = SILVER = new Ore(PREFIX + "Silver", "Серебро", 1.3, 0.1, Ingots.SILVER);
                public static readonly Ore GOLD, Au = GOLD = new Ore(PREFIX + "Gold", "Золото", 3.25, 0.01, Ingots.GOLD);
                public static readonly Ore PLATINUM, Pt = PLATINUM = new Ore(PREFIX + "Platinum", "Платина", 0.433, 0.005, Ingots.PLATINUM);
                public static readonly Ore URANIUM, U = URANIUM = new Ore(PREFIX + "Uranium", "Уран", 0.325, 0.01, Ingots.URANIUM);
                public static readonly Item ICE = new Item(PREFIX + "Ice", "Лёд");

            }

            /// <summary>
            /// Содержит перечисление ресурсов
            /// </summary>
            public static class Ingots
            {
                private static readonly String PREFIX = "MyObjectBuilder_Ingot/";
                public static readonly Item STONE = new Item(PREFIX + "Stone", "Гравий");
                public static readonly Item IRON, Fe = IRON = new Item(PREFIX + "Iron", "Железо");
                public static readonly Item SILICON, Si = SILICON = new Item(PREFIX + "Silicon", "Кремний");
                public static readonly Item NICKEL, Ni = NICKEL = new Item(PREFIX + "Nickel", "Никель");
                public static readonly Item COBALT, Co = COBALT = new Item(PREFIX + "Cobalt", "Кобальт");
                public static readonly Item MAGNESIUM, Mg = MAGNESIUM = new Item(PREFIX + "Magnesium", "Магний");
                public static readonly Item SILVER, Ag = SILVER = new Item(PREFIX + "Silver", "Серебро");
                public static readonly Item GOLD, Au = GOLD = new Item(PREFIX + "Gold", "Золото");
                public static readonly Item PLATINUM, Pt = PLATINUM = new Item(PREFIX + "Platinum", "Платина");
                public static readonly Item URANIUM, U = URANIUM = new Item(PREFIX + "Uranium", "Уран");
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

            public static readonly List<CraftableItem> COMPONENTS = new List<CraftableItem> {
                Components.STEEL_PLATE, Components.INTEROR_PLATE, Components.CONSTRUCTION, Components.COMPUTER, Components.MOTOR,
                Components.GIRDER, Components.SMALL_TUBE, Components.LARGE_TUBE, Components.METAL_GRID, Components.DISPLAY,
                Components.BULLETPROOF_GLASS, Components.POWER_CELL, Components.RADIO_COMMUNICATION, Components.MEDICAL,
                Components.REACTOR, Components.THRUST, Components.DETECTOR, Components.GRAVITY_GENERATOR, Components.EXPLOSIVES,
                Components.SOLAR_CELL, Components.SUPERCONDUCTOR
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

            public static readonly List<CraftableItem> CRAFTABLES = COMPONENTS;

            public static readonly List<Item> ALL = ORES.Concat(INGOTS).Concat(COMPONENTS).Concat(INSTRUMENTS).Concat(OTHER).ToList();
        }

        public class Item
        {

            public readonly MyDefinitionId Id;
            public readonly string Name;

            public Item(string id, string name)
            {
                MyDefinitionId.TryParse(id, out Id);
                Name = name;
            }
        }

        public class ItemStack
        {
            public ItemStack(Item item, double amount)
            {
                Item = item;
                Amount = amount;
            }

            public Item Item { get; set; }
            public double Amount { get; set; }

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


        public class CraftableItem : Item
        {
            public double CraftingTime { get; private set; }
            public List<ItemStack> Ingredients { get; private set; }
            public MyDefinitionId BlueprintId;

            public CraftableItem(string id, string name, double craftingTime, IEnumerable<ItemStack> ingredients) : base(id, name)
            {
                CraftingTime = craftingTime;
                Ingredients = ingredients.ToList();
                var blueprintSubtype = GetBlueprintSubdefinitionName(Id.SubtypeName);

                MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition/" + blueprintSubtype, out BlueprintId);
            }

            public CraftableItem(string id, string name, double craftingTime, object[] ingredients) : base(id, name)
            {

                CraftingTime = craftingTime;
                Ingredients = ToItemStacks(ingredients);
                var blueprintSubtype = GetBlueprintSubdefinitionName(Id.SubtypeName);

                MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition/" + blueprintSubtype, out BlueprintId);
            }

            private static List<ItemStack> ToItemStacks(params object[] items)
            {
                var stacks = new List<ItemStack>();
                for (int i = 0; i < items.Length; i += 2)
                {
                    stacks[i / 2] = new ItemStack((Item)items[i], (double)items[i + 1]);
                }
                return stacks;
            }

            private string GetBlueprintSubdefinitionName(string itemSubdefinition)
            {
                var suffix = "Component";
                var subdefs = new[] { "Motor", "Computer", "Construction", "Detector", "Explosives", "Girder",
                    "GravityGenerator", "Medical", "Thrust", "RadioCommunication", "Reactor" };

                if (subdefs.Contains(itemSubdefinition))
                    return itemSubdefinition + suffix;

                return itemSubdefinition;
            }

            public CraftableItem(string id, string name) : base(id, name)
            {
                CraftingTime = 1;
                Ingredients = new List<ItemStack> { new ItemStack(Items.Ingots.IRON, 20) };
                var blueprintSubtype = GetBlueprintSubdefinitionName(Id.SubtypeName);
                MyDefinitionId.TryParse("MyObjectBuilder_BlueprintDefinition/" + blueprintSubtype, out BlueprintId);
            }

            public CraftableItem WithTime(double time)
            {
                CraftingTime = time;
                return this;
            }

            public CraftableItem WithIngredients(params object[] items)
            {
                var stacks = new List<ItemStack>();
                for (int i = 0; i < items.Length; i += 2)
                {
                    if (items[i] is Item && items[i + 1] is double)
                    {
                        stacks.Add(new ItemStack(items[i] as Item, (double)items[i + 1]));
                    }
                }
                Ingredients = stacks;
                return this;
            }
        }

        // TODO: режим боли
        public class Weapon : CraftableItem
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

        public class Ammo : CraftableItem
        {
            public readonly int MaxAmmo;

            public Ammo(string id, string name, int maxAmmo) : base(id, name)
            {
                MaxAmmo = maxAmmo;
            }
        }
    }
}
