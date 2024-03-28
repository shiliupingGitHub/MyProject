using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Script.Character;
using Game.Script.Common;
using Game.Script.Map;
using Game.Script.Res;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Script.Game
{
    public enum GameMode
    {
        Hall,
        Host,
        Edit,
        Client,
    }
    public class Game : SingletonWithOnInstance<Game>
    {

        public System.Action<MapScript> OnMapBkLoad;
        public System.Action<FightCharacter> OnLocalPlayerLoad;
        public GameMode Mode { set; get; } = GameMode.Host;

        private  Dictionary<System.Type, GameSubsystem> _subsystems = new();
        private List<Pawn> _pawns = new();
        private FightCharacter _myController;
        private MapScript _mapScript;
        private const string KcpNetMgrPath = "Assets/Game/Res/Misc/KcpFightNetworkManager.prefab";
        private GameObject networkMgrGo = null;
        public void RegisterPawn(Pawn pawn)
        {
            if (!_pawns.Contains(pawn))
            {
                _pawns.Add(pawn);
            }
        }
       public void LoadNetWorkManager()
        {
            if (networkMgrGo != null)
            {
                Object.Destroy(networkMgrGo);
            }
            var template = GameResMgr.Instance.LoadAssetSync<GameObject>(KcpNetMgrPath);
            networkMgrGo = Object.Instantiate(template);
        }
        
        public MapScript MapScript
        {
            set
            {
                _mapScript = value;

                if (OnMapBkLoad != null)
                {
                    OnMapBkLoad.Invoke(_mapScript);
                }
            }
            get
            {
                return _mapScript;
            }
        }

        public FightCharacter MyController
        {
            set
            {
                _myController = value;

                if (null != OnLocalPlayerLoad)
                {
                    OnLocalPlayerLoad.Invoke(_myController);
                }
            }
            get => _myController;
        }

        public void Tick()
        {
            foreach (var pawn in _pawns)
            {
                pawn.Tick(Time.unscaledDeltaTime);
            }
        }

        public void UnRegisterPawn(Pawn pawn)
        {
            _pawns.Remove(pawn);
        }

        public T GetSubsystem<T>() where T: GameSubsystem
        {
            var type = typeof(T);
            GameSubsystem ret = null;
            _subsystems.TryGetValue(type, out ret);
            return ret as T;
        }

        public override void OnInstance()
        {
            base.OnInstance();
            var baseType = typeof(GameSubsystem);
            var assem = baseType.Assembly;
            foreach (var type in assem.GetTypes())
            {
                if (baseType.IsAssignableFrom(type) && type != baseType)
                {
                    if (System.Activator.CreateInstance(type) is GameSubsystem subsystem)
                    {
                       
                        _subsystems.Add(type,subsystem);
                    }
                   
                }
            }

            foreach (var subsystem in _subsystems)
            {
                subsystem.Value.OnInitialize();
            }
          
        }
    }
}