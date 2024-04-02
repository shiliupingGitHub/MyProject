using System.Collections.Generic;
using System.Threading.Tasks;
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
        Home,
    }
    public class Game : SingletonWithOnInstance<Game>
    {

        public System.Action<MapBk> mapBkLoad;
        public System.Action<FightCharacter> localPlayerLoad;
        public System.Action<AICharacter, bool> addMonster;
        public System.Action<AICharacter, bool> removeMonster;
        public System.Action fightStart;
        public System.Action serverFightSceneChanged;
        public GameMode Mode { set; get; } = GameMode.Host;
        private bool _fightStart ;
        public string FightMap { get; set; }
        public bool FightStart
        {
            get => _fightStart;
            set
            {
                if (_fightStart != value)
                {
                    _fightStart = value;

                    if (_fightStart)
                    {
                        fightStart?.Invoke();
                    }
                    else
                    {
                        fightStart = null;
                    }
                }
            }
        }

        public List<FightCharacter> Fights { get; } = new();

        private readonly Dictionary<System.Type, GameSubsystem> _subsystems = new();
        private readonly List<Pawn> _pawns = new();
        private FightCharacter _myController;
        private MapBk _mapBk;
        private const string KcpNetMgrPath = "Assets/Game/Res/Net/KcpFightNetworkManager.prefab";
        private GameObject _networkMgrGo;
        private float _lastTickTime = 0;
        
        

        public void RegisterPawn(Pawn pawn)
        {
            if (!_pawns.Contains(pawn))
            {
                _pawns.Add(pawn);
                if (pawn is FightCharacter character)
                {
                    Fights.Add(character);
                }
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
            _networkMgrGo.name = "NetworkMgr";
        }
        
        public MapBk MapBk
        {
            set
            {
                _mapBk = value;

                if (mapBkLoad != null)
                {
                    mapBkLoad.Invoke(_mapBk);
                }
            }
            get => _mapBk;
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
            if (_lastTickTime == 0)
            {
                _lastTickTime = Time.unscaledTime;
            }
            else
            {
                float curTime = Time.unscaledTime;
                float delta = curTime - _lastTickTime;
                foreach (var pawn in _pawns)
                {
                    pawn.Tick(delta);
                }

                _lastTickTime = curTime;
            }
            
        }

         async void DoTick()
        {
            while (true)
            {
                Tick();
                await Task.Delay(1);
            }
        }

        public void UnRegisterPawn(Pawn pawn)
        {
            _pawns.Remove(pawn);
            if (pawn is FightCharacter character)
            {
                Fights.Remove(character);
            }
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

            DoTick();

        }
    }
}