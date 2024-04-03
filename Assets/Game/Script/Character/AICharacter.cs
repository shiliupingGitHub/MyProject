
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Game.Script.AI;
using Game.Script.Async;
using Game.Script.Common;
using UnityEngine;


namespace Game.Script.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class AICharacter : Character
    {
 

        private GameBehaviorTree _gameBehaviorTree;
        public ExternalBehavior externalBehaviorTree;
        public GameBehaviorTree BehaviorTree => _gameBehaviorTree;

        public override void OnStartServer()
        {
            base.OnStartServer();

            _gameBehaviorTree = gameObject.AddComponent<GameBehaviorTree>();
            var etbt = UnityEngine.Object.Instantiate(externalBehaviorTree);
            _gameBehaviorTree.RestartWhenComplete = true;
            _gameBehaviorTree.DisableBehavior();
            _gameBehaviorTree.ExternalBehavior = etbt;
            _gameBehaviorTree.EnableBehavior();
        }

        protected override bool IsBlock => true;
        protected override Vector2Int[] Areas => new[] { Vector2Int.zero };
        
        protected override void Start()
        {
            base.Start();

            if (Common.Game.Instance.addMonster != null)
            {
                Common.Game.Instance.addMonster.Invoke(this, isServer);
            }
        }

        

        protected override void OnDestroy()
        {
            base.OnDestroy();

           

            if (Common.Game.Instance.removeMonster != null)
            {
                Common.Game.Instance.removeMonster.Invoke(this, isServer);
            }
        }
        



  
    }
}