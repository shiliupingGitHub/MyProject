using System;
using System.Collections.Generic;
using Cinemachine;
using Game.Script.Common;
using Game.Script.Map.Actor;
using Game.Script.Res;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Script.Map
{
    [Serializable]
    public class ActorData
    {
        [SerializeField] public int x;
        [SerializeField] public int y;
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

        public void RemoveActor(Vector3 position)
        {
            MapBk mapBk = GameObject.FindObjectOfType<MapBk>();

            if (mapBk == null)
                return ;
            
            (int x, int y) = mapBk.GetGridIndex(position);

            List<ActorData> removes = new();
            foreach (var actor in _actors)
            {
                if (actor.x == x && actor.y == y)
                {
                    removes.Add(actor);
                }
            }

            foreach (var remove in removes)
            {
                _actors.Remove(remove);
                Object.Destroy(remove.go);
            }
        }
        public bool AddActor(Vector3 position, ActorConfig actorConfig, bool preview = true, bool net = false)
        {
            MapBk mapBk = GameObject.FindObjectOfType<MapBk>();

            if (mapBk == null)
                return false;

            
            var template = GameResMgr.Instance.LoadAssetSync<GameObject>(actorConfig.path);

            if (template)
            {
                var go = GameObject.Instantiate(template) as GameObject;
                if (preview)
                {
                    go.tag = "Preview";
                }

                var actor = go.GetComponent<MapActor>();
               
                if (actor != null)
                {
                    actor.Config = actorConfig;
                    actor.ActorType = ActorType.Preview;
                }

                ActorData actorData = new();
                actorData.go = go;
                (int x, int y) = mapBk.GetGridIndex(position);
                actorData.x = x;
                actorData.y = y;
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

                if (Preview)
                {
                    var mapScript = _bkMapGo.GetComponent<MapBk>();
                    
                    mapScript.virtualCamera.gameObject.SetActive(false);
                    var brain =  mapScript.GetComponentInChildren<CinemachineBrain>();
                    brain.gameObject.SetActive(false);
                }
                
            }
        }

        void LoadActor(MapBk mapBk, ActorData actorData, bool preview = true, bool net = false)
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

                var actor = go.GetComponent<MapActor>();

                if (actor != null)
                {
                    actor.Config = actorConfig;
                    actor.ActorType = preview ? ActorType.Preview : ActorType.Normal;
                }

                go.transform.position = mapBk.GetPosition(actorData.x, actorData.y);
                actorData.go = go;

                if (net && actor.isNet)
                {
                    if (go.TryGetComponent(out NetworkIdentity identity))
                    {
                        NetworkServer.Spawn(go);
                    }

                }
            }
        }

        public void LoadActorsSync(bool preview = true, bool net = false)
        {
            MapBk mapBk = Object.FindObjectOfType<MapBk>();

            if (mapBk == null)
                return ;
            foreach (var actorData in _actors)
            {
                LoadActor(mapBk, actorData, preview, net);

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