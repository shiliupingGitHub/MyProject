﻿
using BehaviorDesigner.Runtime;
using Game.Script.AI;

namespace Game.Script.Character
{
    public class AICharacter : Character
    {
        private GameBehaviorTree _gameBehaviorTree;
        public ExternalBehavior _externalBehaviorTree;
        public override void OnStartServer()
        {
            base.OnStartServer();

            _gameBehaviorTree = gameObject.AddComponent<GameBehaviorTree>();
            var etbt = UnityEngine.Object.Instantiate(_externalBehaviorTree) as ExternalBehavior;
            _gameBehaviorTree.DisableBehavior();
            _gameBehaviorTree.ExternalBehavior = etbt;
            _gameBehaviorTree.EnableBehavior();
        }
        
    }
}