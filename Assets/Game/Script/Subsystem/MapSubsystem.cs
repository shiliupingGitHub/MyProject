using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Script.Map;
using Game.Script.Res;
using Game.Script.UI;
using Game.Script.UI.Frames;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Game.Script.Subsystem
{
    public class MapSubsystem : GameSubsystem
    {
        
        string AssetMapPath => "Assets/Game/Res/Map/Data/";

        string MapExtension => ".txt";
        private MapScript _mapScript;

        private readonly Dictionary<uint, MapArea> _areas = new();

        public override void OnInitialize()
        {
            base.OnInitialize();
            Common.Game.Instance.localPlayerLoad += (_) =>
            {
                CheckMap();
            } ;

            Common.Game.Instance.mapBkLoad += script =>
            {
                _mapScript = script;
                CheckMap();
                GenerateInitAreas();
            };
        }


        public MapArea GetArea(int x, int y, bool create = false)
        {
            uint areaKey = CreateAreaIndex((uint)x, (uint)y);

            if (!_areas.TryGetValue(areaKey, out var ret))
            {
                ret = new MapArea();
                _areas.Add(areaKey, ret);
            }
            return ret;
        }

        void CheckMap()
        {
            if (Common.Game.Instance.MapScript != null && Common.Game.Instance.MyController != null)
            {
                var tr = Common.Game.Instance.MyController.transform;
                Common.Game.Instance.MapScript.virtualCamera.Follow = tr;
                Common.Game.Instance.MapScript.virtualCamera.LookAt = tr;
                Common.Game.Instance.MapScript.virtualCamera.gameObject.SetActive(true);
                HideLoading();
            }
        }

        async void HideLoading()
        {
            await Task.Delay(1);
            UIManager.Instance.Hide<LoadingFrame>();
        }
        public MapData New(int bkId)
        {
            MapData mapData = new MapData
            {
                bkId = bkId
            };

            return mapData;
        }

        uint CreateAreaIndex(uint x, uint y)
        {
            x = x << 16;

            uint ret = x | y;

            return ret;

        }
        public (int, int, int) CreateAreaIndex(Vector3 position)
        {
            if (_mapScript == null)
            {
                return (-1, -1, -1);
            }

            
            Vector3 relative = position - _mapScript.transform.position;

            int x = Mathf.FloorToInt(relative.x / _mapScript.MyGrid.cellSize.x);
            int y = Mathf.FloorToInt(relative.y / _mapScript.MyGrid.cellSize.y);

            if (x < 0)
            {
                return (-1, -1, -1);;
            }

            if (y < 0)
            {
                return (-1, -1, -1);;
            }
            
            return ( (int)CreateAreaIndex((uint)x, (uint)y), x, y);
        }

        void AddAreaMapBlock(uint x, uint y)
        {
            uint areaIndex = CreateAreaIndex(x, y);
            if (!_areas.TryGetValue(areaIndex, out var area))
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

            var tileMaps = _mapScript.blockTilesRoot.GetComponentsInChildren<Tilemap>();

            foreach (var tilemap in tileMaps)
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
        }
    }
}