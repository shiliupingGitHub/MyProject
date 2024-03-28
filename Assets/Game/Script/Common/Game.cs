using System.Collections.Generic;
using Game.Script.Character;
using Game.Script.Map;
using Game.Script.Res;
using Game.Script.Subsystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Script.Common
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

        public System.Action<MapScript> mapBkLoad;
        public System.Action<FightCharacter> localPlayerLoad;
        public GameMode Mode { set; get; } = GameMode.Host;

        private readonly Dictionary<System.Type, GameSubsystem> _subsystems = new();
        private readonly List<Pawn> _pawns = new();
        private FightCharacter _myController;
        private MapScript _mapScript;
        private const string KcpNetMgrPath = "Assets/Game/Res/Misc/KcpFightNetworkManager.prefab";
        private GameObject _networkMgrGo;
        
        public void RegisterPawn(Pawn pawn)
        {
            if (!_pawns.Contains(pawn))
            {
                _pawns.Add(pawn);
            }
        }
        
       public void LoadNetWorkManager()
        {
            if (_networkMgrGo != null)
            {
                Object.Destroy(_networkMgrGo);
            }
            var template = GameResMgr.Instance.LoadAssetSync<GameObject>(KcpNetMgrPath);
            _networkMgrGo = Object.Instantiate(template);
        }
        
        public MapScript MapScript
        {
            set
            {
                _mapScript = value;

                if (mapBkLoad != null)
                {
                    mapBkLoad.Invoke(_mapScript);
                }
            }
            get => _mapScript;
        }

        public FightCharacter MyController
        {
            set
            {
                _myController = value;

                if (null != localPlayerLoad)
                {
                    localPlayerLoad.Invoke(_myController);
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
            _subsystems.TryGetValue(type, out var ret);
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