using Game.Script.Common;
using Game.Script.Game;
using Game.Script.Res;
using UnityEngine;

namespace Game.Script.Map
{
    public class MapMgr : Singleton<MapMgr>
    {
        
        public void Init()
        {
            GameInstance.Instance.OnLocalPlayerLoad += (controller) =>
            {
                CheckMap();
            } ;

            GameInstance.Instance.OnMapBkLoad += script =>
            {
                CheckMap();
            };
        }

        void CheckMap()
        {
            if (GameInstance.Instance.MapScript != null && GameInstance.Instance.MyController != null)
            {
                var tr = GameInstance.Instance.MyController.transform;
                GameInstance.Instance.MapScript.virtualCamera.Follow = tr;
                GameInstance.Instance.MapScript.virtualCamera.LookAt = tr;
            }
        }
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