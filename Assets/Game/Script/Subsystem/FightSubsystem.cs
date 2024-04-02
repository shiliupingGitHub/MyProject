using Game.Script.Common;
using UnityEditor.VersionControl;

namespace Game.Script.Subsystem
{
    public class FightSubsystem : GameSubsystem
    {
        public System.Action fightStart;
        private bool _fightStart;
        private float _startLeftTime = 5;
        private bool _prepareStart = false;

        public bool FightStart
        {
            get => _fightStart;
            set
            {
                _fightStart = value;

                if (_fightStart)
                {
                    fightStart?.Invoke();
                }
                else
                {
                    fightStart = null;
                    _prepareStart = false;
                }
            }
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            GameLoop.Add(OnUpdate);
        }

        void OnUpdate(float deltaTime)
        {
            if (!_fightStart)
            {
                if (_prepareStart)
                {
                    _startLeftTime -= deltaTime;

                    if (_startLeftTime <= 0)
                    {
                        _prepareStart = false;
                        FightStart = true;
                    }
                }
            }
        }

        public void StartFight()
        {
            _startLeftTime = 5;
            _prepareStart = true;
        }
    }
}