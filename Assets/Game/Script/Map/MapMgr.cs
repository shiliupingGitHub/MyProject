using System.Collections.Generic;
using Game.Script.Common;
using Game.Script.Game;
using Game.Script.Res;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Script.Map
{
    public class MapMgr : Singleton<MapMgr>
    {
        
        string AssetMapPath => "Assets/Game/Res/Map/Data/";

        string MapExtension => ".txt";
        private MapScript _mapScript;

        private Dictionary<uint, MapArea> _areas = new();
        
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

        public MapArea GetArea(int x, int y)
        {
            uint areaKey = CreateAreaIndex((uint)x, (uint)y);

            MapArea ret = null;

            _areas.TryGetValue(areaKey, out ret);

            return ret;
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

        uint CreateAreaIndex(uint x, uint y)
        {
            x = x << 16;

            uint ret = x | y;

            return ret;

        }

        void AddAreaMapBlock(uint x, uint y)
        {
            uint areaIndex = CreateAreaIndex(x, y);
            MapArea area = null;
            if (!_areas.TryGetValue(areaIndex, out area))
            {
                area = new MapArea();
                _areas.Add(areaIndex, area);
            }

            area.MapBlocked = true;
        }   
        void GenerateInitAreas()
        {
            _areas.Clear();

            if (_mapScript == null)
            {
                return;
            }
            
            if(_mapScript.blockTilesRoot == null)
                return;

            var tilemaps = _mapScript.blockTilesRoot.GetComponentsInChildren<Tilemap>();

            foreach (var tilemap in tilemaps)
            {
                var bound = tilemap.cellBounds;
                foreach (var pos in bound.allPositionsWithin)
                {
                    var sp = tilemap.GetSprite(pos);

                    if (null != sp)
                    {
                        if (pos.x >= 0
                            && pos.x < _mapScript.xGridNum
                            && pos.y >= 0
                            && pos.y < _mapScript.yGridNum
                           )
                        {
                            AddAreaMapBlock((uint)pos.x, (uint)pos.y);
                        }
                    }
                }
            }


        }

        
        public void LoadMap(string mapName, bool net, bool inAsset = true)
        {
            var path = AssetMapPath + mapName + MapExtension;

            var content = GameResMgr.Instance.LoadAssetSync<TextAsset>(path);
            var mapData = MapData.DeSerialize(content.text);
            
            mapData.LoadSync(false, true);
            _mapScript = GameObject.FindObjectOfType<MapScript>();
            GenerateInitAreas();

        }
    }
}