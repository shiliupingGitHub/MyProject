using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BehaviorDesigner.Runtime;
using Game.Script.AI;
using Game.Script.Common;
using UnityEngine;


namespace Game.Script.Character
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class AICharacter : Character
    {
        public enum PathState
        {
            None,
            Moving,
            Success,
            Fail,
        }

        private GameBehaviorTree _gameBehaviorTree;
        public ExternalBehavior externalBehaviorTree;
        public float moveSpeed = 100;
        private Rigidbody2D _rigidbody;
        public PathState CurPathState { get; set; } = PathState.None;
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
        private List<Vector3> _path;
        private int _curPathIndex = -1;
        private float _curAcceptRadius = 1f;

        private GameObject _targetGo = null;
        private TaskCompletionSource<PathState> _pathTcl;

        public Task<PathState> SetPath(List<Vector3> path, float acceptRadius = 1.2f, GameObject targetGo = null)
        {
            _pathTcl = new TaskCompletionSource<PathState>();
            _curPathIndex = 2;
            _path = path;
            CurPathState = PathState.Moving;
            _curAcceptRadius = 1;
            _targetGo = targetGo;


            return _pathTcl.Task;
        }

        protected override void Start()
        {
            base.Start();

            if (Common.Game.Instance.addMonster != null)
            {
                Common.Game.Instance.addMonster.Invoke(this, isServer);
            }
        }


        protected override void Awake()
        {
            base.Awake();

            GameLoop.AddFixed(OnFixedUpdate);
            _rigidbody = GetComponent<Rigidbody2D>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            GameLoop.RemoveFixed(OnFixedUpdate);

            if (Common.Game.Instance.removeMonster != null)
            {
                Common.Game.Instance.removeMonster.Invoke(this, isServer);
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (CurPathState == PathState.Moving)
            {
                CurPathState = other.gameObject == _targetGo ? PathState.Success : PathState.Fail;
                _rigidbody.velocity = Vector3.zero;

                if (null != _pathTcl)
                {
                    _pathTcl.SetResult(CurPathState);
                }
            }

            _curPathIndex = -1;
            _path = null;
            _pathTcl = null;
        }


        void OnFixedUpdate(float deltaTime)
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
                CurPathState = PathState.Success;

                if (null != _pathTcl)
                {
                    _pathTcl.SetResult(CurPathState);
                }

                return;
            }

            var targetPosition = _path[_curPathIndex];
            var curPosition = transform.position;
            var dir = targetPosition - curPosition;

            if (dir.sqrMagnitude < 0.01)
            {
                _rigidbody.velocity = Vector3.zero;
                _curPathIndex++;
            }
            else
            {
                var endPosition = _path[^1];
                if (_curAcceptRadius >= Vector3.Distance(curPosition, endPosition))
                {
                    CurPathState = PathState.Success;
                    _rigidbody.velocity = Vector3.zero;
                    _path = null;
                    _curPathIndex = -1;
                    if (null != _pathTcl)
                    {
                        _pathTcl.SetResult(CurPathState);
                    }
                }
                else
                {
                    var dis = Vector3.Distance(targetPosition, curPosition);
                    float pathSpeed = dis / Time.fixedDeltaTime;
                    float speed = Mathf.Min(pathSpeed, moveSpeed);

                    _rigidbody.velocity = dir.normalized * speed;
                }
            }
        }
    }
}