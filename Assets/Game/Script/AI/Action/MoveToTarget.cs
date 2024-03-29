using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Game.Script.Character;
using Game.Script.Subsystem;

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
                var pathSystem = Common.Game.Instance.GetSubsystem<PathSubsystem>();
                var end = target.Value.transform.position;
                var start = gameObject.transform.position;
                _pathId = pathSystem.AddPath(start, end);
                _moveStatus = MoveStatus.Path;
            }
            else
            {
                _moveStatus = MoveStatus.Fail;
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
                case MoveStatus.Path:
                {
                    var pathSystem = Common.Game.Instance.GetSubsystem<PathSubsystem>();
                    var path = pathSystem.GetPath(_pathId);

                    if (path == null)
                        return TaskStatus.Running;

                    if (path.Count == 0)
                        return TaskStatus.Failure;

                    var character = GetComponent<AICharacter>();

                    character.SetPath(path, acceptRadius, target.Value);
                    pathSystem.RemovePath(_pathId);
                    _moveStatus = MoveStatus.Moving;
                }
                    return TaskStatus.Running;
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