using Game.Script.Common;
using Game.Script.Res;
using UnityEngine;

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

         string AssetMapPath => "Assets/Game/Res/Map/Data/";

         string MapExtension => ".txt";

        public void LoadMap(string mapName, bool net, bool inAsset = true)
        {
            var path = AssetMapPath + mapName + MapExtension;

            var content = GameResMgr.Instance.LoadAssetSync<TextAsset>(path);
            var mapData = MapData.DeSerialize(content.text);
            
            mapData.LoadSync(false, true);
        }
    }
}