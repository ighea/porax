using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PORAx
{

    public struct StageItem
    {
        public string xmlDescriptor;
        public bool isMap;
    }

    public class MapperData
    {
        public float stageEndDelay;
        public StageItem[] stages;
    }
}
