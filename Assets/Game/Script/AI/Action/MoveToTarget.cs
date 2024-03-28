using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace Game.Script.AI.Action
{
    public class MoveToTarget : BehaviorDesigner.Runtime.Tasks.Action
    {
        public SharedGameObject target;

        enum MoveStatus
        {
            None,
            Success,
            Fail,
            Path,
            Moving,
        }

        private MoveStatus _moveStatus = MoveStatus.None;
        public override void OnStart()
        {
            base.OnStart();

            if (target.Value != null)
            {
                
            }
        }

        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Success;
        }
    }
}