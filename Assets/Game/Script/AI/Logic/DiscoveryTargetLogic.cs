using BehaviorDesigner.Runtime;
using Game.Script.Character;

namespace Game.Script.AI.Logic
{
    public class DiscoveryTargetLogic : AILogic
    {
        public override void Tick(AICharacter character, float deltaTime)
        {
            if (Common.Game.Instance.MyController != null)
            {
                if (character.BehaviorTree != null)
                {
                    var targetVar = character.BehaviorTree.GetVariable("Target") as SharedGameObject;

                    if (null != targetVar && targetVar.Value == null)
                    {
                        character.BehaviorTree.SetVariableValue("Target", Common.Game.Instance.MyController.gameObject);
                    }
                }
                
            }
        }
    }
}