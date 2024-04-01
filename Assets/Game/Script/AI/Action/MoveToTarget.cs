using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.Script.Character;
using Game.Script.Subsystem;
using UnityEngine;

namespace Game.Script.AI.Action
{
    public class MoveToTarget : BehaviorDesigner.Runtime.Tasks.Action
    {
        enum MoveStatus
        {
            None,
            Success,
            Fail,
            Path,
            Moving,
        }

        public SharedGameObject target;
        public float acceptRadius = 1.0f;
        private MoveStatus _moveStatus = MoveStatus.None;
        private ulong _pathId;

        public override void OnStart()
        {
            base.OnStart();

            if (target.Value != null)
            {
                _moveStatus = MoveStatus.Path;
                var end = target.Value.transform.position;
                var start = gameObject.transform.position;
                FindPath(start, end);
            }
            else
            {
                _moveStatus = MoveStatus.Fail;
            }
        }

        async void FindPath(Vector3 start, Vector3 end)
        {
            var pathSystem = Common.Game.Instance.GetSubsystem<PathSubsystem>();
            var path = await pathSystem.AddPath(start, end, ref _pathId);
            DoPath(path);
        }

        async void DoPath(List<Vector3> path)
        {
            if (_moveStatus != MoveStatus.Path)
            {
                return;
            }

            if (path.Count == 0)
            {
                _moveStatus = MoveStatus.Fail;
                return;
            }

            var character = GetComponent<AICharacter>();
            _pathId = 0;
            _moveStatus = MoveStatus.Moving;
            var result = await character.SetPath(path, acceptRadius, target.Value);

            DoResult(result);
        }

        void DoResult(AICharacter.PathState result)
        {
            if (_moveStatus == MoveStatus.Moving)
            {
                _moveStatus = ConvertPathState(result);
            }
        }

        public override void OnEnd()
        {
            base.OnEnd();
            _moveStatus = MoveStatus.None;
            if (_pathId > 0)
            {
                var pathSystem = Common.Game.Instance.GetSubsystem<PathSubsystem>();
                pathSystem.RemovePath(_pathId);
            }
        }

        MoveStatus ConvertPathState(AICharacter.PathState state)
        {
            switch (state)
            {
                case AICharacter.PathState.Fail:
                    return MoveStatus.Fail;
            }

            return MoveStatus.Success;
        }

        public override TaskStatus OnUpdate()
        {
            switch (_moveStatus)
            {
                case MoveStatus.Fail:
                {
                    return TaskStatus.Failure;
                }
                case MoveStatus.Success:
                {
                    return TaskStatus.Success;
                }

                default:
                    return TaskStatus.Running;
            }
        }
    }
}