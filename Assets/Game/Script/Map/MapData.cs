using System;
using System.Collections.Generic;
using Game.Script.Res;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Script.Map
{
    [Serializable]
    public class ActorData
    {
        [SerializeField]
        public Vector3 offset;
        [SerializeField]
        public int id;
        [NonSerialized]
        public GameObject go;
    }
    public class MapData
    {
        [SerializeField]
        public int bkId;
        [SerializeField]   List<ActorData> _actors = new();
        [NonSerialized]
        private GameObject _bkMapGo;
        public GameObject BkMapGo => _bkMapGo;
        
        public void LoadSync(bool preview = true,bool net = false)
        {
            LoadBk(preview, net);
            LoadActorsSync(preview, net);
        }

        public string Serialize()
        {
            return JsonUtility.ToJson(this);
        }

        public static MapData DeSerialize(string data)
        {
           return JsonUtility.FromJson<MapData>(data);
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
                _actors.Add(actorData);
                go.transform.position = position;


            }
            return false;
        }

        public void LoadBk(bool Preview, bool net)
        {
            if (MapBKConfig.dic.ContainsKey(bkId))
            {
                var template = GameResMgr.Instance.LoadAssetSync<GameObject>(MapBKConfig.dic[bkId].path);
                _bkMapGo = Object.Instantiate(template) as GameObject;
                
                _bkMapGo.transform.localPosition = Vector3.zero;

                if (net)
                {
                    NetworkServer.Spawn(_bkMapGo);
                }
                
            }
        }

        public void LoadActorsSync(bool preview = true, bool net = false)
        {
            MapScript mapScript = Object.FindObjectOfType<MapScript>();

            if (mapScript == null)
                return ;
            foreach (var actorData in _actors)
            {
                ActorConfig actorConfig = ActorConfig.dic[actorData.id];
                var template = GameResMgr.Instance.LoadAssetSync<GameObject>(actorConfig.path);

                if (template)
                {
                    var go = Object.Instantiate(template) as GameObject;
                    if (preview)
                    {
                        go.tag = "Preview";
                    }
                    
                    go.transform.position = mapScript.Offset + actorData.offset;
                    actorData.go = go;

                    if (net)
                    {
                        if (go.TryGetComponent(out NetworkIdentity identity))
                        {
                            NetworkServer.Spawn(go);
                        }
                        
                    }

                }
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

        public void UnLoadActors()
        {
            foreach (var actorData in _actors)
            {
                if (actorData.go != null)
                {
                    Object.Destroy(actorData.go);
                }
            }
            _actors.Clear();
        }

        public void UnLoadSync()
        {
            UnLoadBk();
            UnLoadActors();
        }
    }
}