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
    /// Скрипт, в который подключаются все библиотеки, чтобы в них заработал анализатор.
    /// Если миксин (Lib) не подключить как зависимость хотя бы к какому-нибудь Script, то анализ кода не работает.
    /// Поэтому во время разработки библиотеки, когда ещё нет скрипта, использующего её, можно подключать либу к этому скрипту как зависимость.
    /// Лучше подключать сюда все библиотеки и оставить их тут как зависимость.
    /// </summary>
    partial class Program : MyGridProgram
    {
        

        public Program()
        {

        }

        public void Save()
        {
            
        }

        public void Main(string argument, UpdateType updateSource)
        {
            
        }
    }
}
