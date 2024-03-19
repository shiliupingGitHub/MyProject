using System.Collections.Generic;
using Game.Script.Res;
using UnityEngine;

namespace Game.Script.Map
{
    public class ActorData
    {
        public int x = 0;
        public int y = 0;
        public string id;
    }
    public class MapData
    {
        public int bkId;
        public List<ActorData> mapActorConfigs;

        private GameObject _bkMapGo;
        public void LoadSync(bool net = false)
        {
            LoadBk();
        }

        public void LoadBk()
        {
            if (MapConfig.dic.ContainsKey(bkId))
            {
                var template = GameResMgr.Instance.LoadAssetSync<GameObject>(MapConfig.dic[bkId].path);
                _bkMapGo = Object.Instantiate(template) as GameObject;
                
                _bkMapGo.transform.localPosition = Vector3.zero;
            }
        }

        public void UnLoadBk()
        {
            if (_bkMapGo != null)
            {
                if (Application.isPlaying)
                {
                    Object.Destroy(_bkMapGo);
                }
                else
                {
                    Object.DestroyImmediate(_bkMapGo);
                }
            }
        }

        public void UnLoadSync()
        {
            UnLoadBk();
        }
    }
}