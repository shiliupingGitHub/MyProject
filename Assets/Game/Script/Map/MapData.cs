using System.Collections.Generic;
using UnityEngine;

namespace Game.Script.Map
{
    public class MapActorConfig
    {
        public int x = 0;
        public int y = 0;
        public string id;
    }
    public class MapData
    {
        private string _bkId;
        private List<MapActorConfig> _mapActorConfigs;
    }
}