﻿using Game.Script.Common;
using UnityEditor.VersionControl;

namespace Game.Script.Subsystem
{
    public class FightSubsystem : GameSubsystem
    {
        public System.Action fightStart;
        public System.Action startLeftTimeChanged;
        private float _startLeftTime = 0;

        public float StartLeftTime
        {
            get => _startLeftTime;
            set
            {
                _startLeftTime = value;
                startLeftTimeChanged?.Invoke();
            }
        }

        private bool _fightStart;
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
                    StartLeftTime = 0;
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
            if (StartLeftTime > 0)
            {
                StartLeftTime -= deltaTime;

                if (StartLeftTime <= 0)
                {
                    StartLeftTime = 0;

                    if (!_fightStart)
                    {
                        if (_prepareStart)
                        {
                            StartLeftTime -= deltaTime;
                            _prepareStart = false;
                            FightStart = true;
                        }
                    }
                }
            }
        }

        public void StartFight()
        {
            StartLeftTime = 5;
            _prepareStart = true;
        }
    }
}