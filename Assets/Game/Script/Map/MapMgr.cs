using Game.Script.Common;

namespace Game.Script.Map
{
    public class MapMgr : Singleton<MapMgr>
    {

        public MapData New(int bkId)
        {
            MapData mapData = new MapData();

            mapData.bkId = bkId;

            return mapData;
        }
    }
}