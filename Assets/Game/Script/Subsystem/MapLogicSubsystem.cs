using System.Collections.Generic;
using System.Threading.Tasks;
using Game.Script.Common;
using Game.Script.Map.Logic;

namespace Game.Script.Subsystem
{
    public class MapLogicSubsystem : GameSubsystem
    {
        private bool _bDoLogic;
        private readonly List<MapLogic> _logics = new();
        public override void OnInitialize()
        {
            base.OnInitialize();
            GameLoop.Add(OnUpdate);
            var baseType = typeof(MapLogic);
            var types = baseType.Assembly.GetTypes();
            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && baseType != type)
                {
                    var logic = System.Activator.CreateInstance(type) as MapLogic;
                    _logics.Add(logic);
                }
            }
            var gameEventSubsystem = Common.Game.Instance.GetSubsystem<EventSubsystem>();
            gameEventSubsystem.Subscribe("AllMapLoaded", _ =>
            {
                _bDoLogic = true;
            });
            gameEventSubsystem.Subscribe("LeaveMap", _ =>
            {
                _bDoLogic = false;
            });
        }

        void OnUpdate(float deltaTime)
        {
            if (_bDoLogic)
            {
                int num = _logics.Count;
                Parallel.For(0, num, (i, _) =>
                {
                    var logic = _logics[i];
                    logic.Tick(deltaTime);
                });
            }
        }
    }
}