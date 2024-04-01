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
           
           if (path.Count == 0)
           {
               _moveStatus = MoveStatus.Fail;
               return;
           }
           var character = GetComponent<AICharacter>();
           _pathId = 0;
           _moveStatus = MoveStatus.Moving;
           character.SetPath(path, acceptRadius, target.Value);
         
        }

        public override void OnEnd()
        {
            base.OnEnd();
            if (_pathId > 0)
            {
                var pathSystem = Common.Game.Instance.GetSubsystem<PathSubsystem>();
                pathSystem.RemovePath(_pathId);
            }
        }

        TaskStatus ConvertPathState(AICharacter.PathState state)
        {
            switch (state)
            {
                case AICharacter.PathState.Success:
                    return TaskStatus.Success;
                case AICharacter.PathState.Fail:
                    return TaskStatus.Failure;
            }
            return TaskStatus.Running;
        }

        public override TaskStatus OnUpdate()
        {
            switch (_moveStatus)
            {
                case MoveStatus.Fail:
                {
                    return TaskStatus.Failure;
                }
                case MoveStatus.Moving:
                {
                    var character = GetComponent<AICharacter>();

                    return ConvertPathState(character.CurPathState);
                }
                
                default:
                    return TaskStatus.Running;
            }
        }
    }
}