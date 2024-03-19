using System.Collections.Generic;
using Game.Script.Res;
using UnityEngine;

namespace Game.Script.Map
{
    public struct ActorData
    {
        public Vector3 offset;
        public int id;
        public GameObject go;
    }
    public class MapData
    {
        public int bkId;
        public List<ActorData> actors = new();

        private GameObject _bkMapGo;
        public void LoadSync(bool preview = true,bool net = false)
        {
            LoadBk();
        }

        public bool AddActor(Vector3 position, ActorConfig actorConfig, bool preview = true, bool net = false)
        {
            MapScript mapScript = GameObject.FindObjectOfType<MapScript>();

            if (mapScript == null)
                return false;

            
            var template = GameResMgr.Instance.LoadAssetSync<GameObject>(actorConfig.path);

            if (template)
            {
                var go = GameObject.Instantiate(template) as GameObject;
                if (preview)
                {
                    go.tag = "Preview";
                }

                ActorData actorData = new();
                actorData.go = go;
                actorData.offset = position - mapScript.Offset;
                actorData.id = actorConfig.id;
                actors.Add(actorData);
               go.transform.position = position;


            }
            return false;
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