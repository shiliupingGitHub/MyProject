using BehaviorDesigner.Runtime;
using Game.Script.Character;
using UnityEngine;

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
                    if (character.BehaviorTree.GetVariable("Target") is SharedGameObject targetVar && targetVar.Value == null)
                    {
                        var curDisSqt = float.MaxValue;
                        var mePosition = character.transform.position;
                        FightCharacter player = null;
                        foreach (var fight in Common.Game.Instance.Fights)
                        {
                            var fightPosition = fight.transform.position;
                            var tempSqt = (fightPosition - mePosition).sqrMagnitude;
                            if (tempSqt < curDisSqt)
                            {
                                curDisSqt = tempSqt;
                                player = fight;

                            }
                        }

                        if (player != null)
                        {
                            character.BehaviorTree.SetVariableValue("Target", player.gameObject);
                        }
                       
                    }
                }
                
            }
        }
    }
}