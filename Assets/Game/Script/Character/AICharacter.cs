
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Game.Script.AI;
using Game.Script.Common;
using UnityEngine;



namespace Game.Script.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class AICharacter : Character
    {
        private GameBehaviorTree _gameBehaviorTree;
        public ExternalBehavior externalBehaviorTree;
       public float moveSpeed = 100;
        private Rigidbody2D _rigidbody;
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
        protected override Vector2Int[] Areas => new[]{Vector2Int.zero};
        private List<Vector3> _path;
        private int _curPathIndex = -1;

        public void SetPath(List<Vector3> path)
        {
            _curPathIndex = 0;
            _path = path;
        }
        protected override void Awake()
        {
            base.Awake();
            if (Application.isPlaying)
            {
                GameLoop.Instance.doUpdate += OnUpdate;
                _rigidbody = GetComponent<Rigidbody2D>();
            }
        
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Application.isPlaying)
            {
                GameLoop.Instance.doUpdate -= OnUpdate;
            }
            
        }

        void OnUpdate(float deltaTime)
        {
            DoMove(deltaTime);
        }

        void DoMove(float deltaTime)
        {
            if (null == _path)
            {
                _rigidbody.velocity = Vector3.zero;
                return;
            }

            if (_curPathIndex < 0)
            {
                _rigidbody.velocity = Vector3.zero;
                return;
            }

            if (_curPathIndex >= _path.Count)
            {
                _rigidbody.velocity = Vector3.zero;
                return;
            }

            var targetPosition = _path[_curPathIndex];
            var curPosition = transform.position ;
            var dir = targetPosition - curPosition;

            if (dir.sqrMagnitude < 0.1)
            {
                _rigidbody.velocity = Vector3.zero;
                _curPathIndex++;
            }
            else
            {
                var dis = Vector3.Distance(targetPosition, curPosition);
                float pathSpeed = dis / deltaTime;
                float speed = Mathf.Min(pathSpeed, moveSpeed);

                _rigidbody.velocity = dir.normalized * speed;
            }
     
            



        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            if (Common.Game.Instance.MyController != null)
            {
                _gameBehaviorTree.SetVariableValue("Target", Common.Game.Instance.MyController.gameObject);
            }
        }
    }
}