using System.Collections.Generic;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class MonitorCreatorConfig
        {
            public string gridPrefix;

            public string displayPrefix;
            public Color fontColor;
            public Color backgroundColor;
            public ProgressbarSettings progressbarSettings;

            public bool oresMonitorEnable;
            public IDisplay oresDisplay;
            public long oresMaxCount;

            public bool ingotsMonitorEnable;
            public IDisplay ingotsDisplay;
            public long ingotMaxCount;

            public bool componentsMonitorEnable;
            public IDisplay componentsDisplay1;
            public IDisplay componentsDisplay2;
            public Dictionary<Item, long> componentsCountByItem;

            public bool refinesMonitorEnable;
            public IDisplay refinesDisplay;
            public Dictionary<Ore, int> refinesCountByOres;
            public int refinesUniversalCount;

            public bool assemblersMonitorEnable;
            public IDisplay assemblersDisplay;

            public bool containersMonitorEnable;
            public IDisplay containersDisplay;

            public bool hydrogenMonitorEnable;
            public IDisplay hydrogenDisplay;
            public string hydrogenTankTypeKeyword;

            public bool oxygenMonitorEnable;
            public IDisplay oxygenDisplay;
            public string oxygenTankTypeKeyword;

            public bool batteriesMonitorEnable;
            public IDisplay batteriesDisplay;
        }
    }
}