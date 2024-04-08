﻿using System;
using System.Collections.Generic;
using System.Reflection;
using Cinemachine;
using Game.Script.Async;
using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Map.Actor;
using Game.Script.Res;
using Game.Script.Subsystem;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Script.Map
{
    [Serializable]
    public class ActorFieldData
    {
        public string fieldName;
        public string fieldValue;
    }

    [Serializable]
    public class ActorData
    {
        [SerializeField] public int x;
        [SerializeField] public int y;
        [SerializeField] public int id;
        [SerializeField] public List<ActorFieldData> fieldData = new();
        [NonSerialized] public GameObject go;

        public void Set<T>(string fieldName, T value)
        {
            fieldData.RemoveAll(data => data.fieldName == fieldName);
            ActorFieldData data = new();
            data.fieldName = fieldName;
            data.fieldValue = value.ToString();
            fieldData.Add(data);
        }
    }

    [Serializable]
    public class MapActionData
    {
        [SerializeField] public string data;
        [SerializeField] public string name;
        [SerializeField] public MapActionType type;
    }

    [Serializable]
    public class MapEventData
    {
        [SerializeField] public string name;
        [SerializeField] public List<MapActionData> actions = new();
    }

    [Serializable]
    public class MapTimeEventData : MapEventData
    {
        [SerializeField] public float time;
    }

    [Serializable]
    public class MapSystemEventData : MapEventData
    {
    }

    [Serializable]
    public class MapCustomEventData : MapEventData
    {
    }

    public class MapData
    {
        [SerializeField] public int bkId;
        [SerializeField] public List<ActorData> actors = new();
        [SerializeField] public List<MapTimeEventData> timeEvents = new();
        [SerializeField] public List<MapSystemEventData> systemEvents = new();
        [SerializeField] public List<MapCustomEventData> customEvents = new();
        [NonSerialized] private GameObject _bkMapGo;
        public GameObject BkMapGo => _bkMapGo;

        public void LoadSync(bool preview = true, bool net = false)
        {
            LoadBk(preview, net);
            LoadActorsSync(preview, net);
            if (!preview)
            {
                var gameEventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
                if (null != gameEventSubsystem)
                {
                    gameEventSubsystem.Raise("AllMapLoaded", this);
                }
            }
        }


        public async void LoadAsync()
        {
            LoadBk(false, true);

            MapBk mapBk = Object.FindObjectOfType<MapBk>();

            foreach (var actorData in actors)
            {
                LoadActor(mapBk, actorData, false, true);
                await TimerSubsystem.Delay(1);
            }


            var gameEventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
            if (null != gameEventSubsystem)
            {
                gameEventSubsystem.Raise("AllMapLoaded", this);
            }
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
                return;

            (int x, int y) = mapBk.GetGridIndex(position);

            List<ActorData> removes = new();
            foreach (var actor in actors)
            {
                if (actor.x == x && actor.y == y)
                {
                    removes.Add(actor);
                }
            }

            foreach (var remove in removes)
            {
                actors.Remove(remove);
                Object.Destroy(remove.go);
            }
        }

        public ActorData GetActorData(Vector3 position)
        {
            MapBk mapBk = GameObject.FindObjectOfType<MapBk>();

            if (mapBk == null)
                return null;

            (int x, int y) = mapBk.GetGridIndex(position);

            List<ActorData> removes = new();
            foreach (var actor in actors)
            {
                if (actor.x == x && actor.y == y)
                {
                    return actor;
                }
            }

            return null;
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
                actors.Add(actorData);
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
                    var brain = mapScript.GetComponentInChildren<CinemachineBrain>();
                    brain.gameObject.SetActive(false);
                }
            }
        }

        void DeserializeActorField(MapActor actor, ActorData actorData)
        {
            var typeInfo = (System.Reflection.TypeInfo)actor.GetType();

            foreach (var field in typeInfo.DeclaredFields)
            {
                if (field.IsStatic)
                {
                    continue;
                }

                if (!field.IsPublic)
                {
                    continue;
                }

                var fieldType = field.FieldType;

                var attr = field.GetCustomAttribute<ActorDataDesAttribute>();

                if (null != attr)
                {
                    var data = actorData.fieldData.Find(x => x.fieldName == field.Name);

                    if (null != data)
                    {
                        if (fieldType == typeof(int))
                        {
                            field.SetValue(actor, int.Parse(data.fieldValue));
                        }
                        else if (fieldType == typeof(float))
                        {
                            field.SetValue(actor, float.Parse(data.fieldValue));
                        }
                        else if (fieldType == typeof(string))
                        {
                            field.SetValue(actor, data.fieldValue);
                        }
                    }
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
                    DeserializeActorField(actor, actorData);
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
                return;
            foreach (var actorData in actors)
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
            foreach (var actorData in actors)
            {
                if (actorData.go != null)
                {
                    Object.Destroy(actorData.go);
                }
            }

            actors.Clear();
        }

        public void UnLoadSync()
        {
            UnLoadBk();
            UnLoadActors();
        }
    }
}