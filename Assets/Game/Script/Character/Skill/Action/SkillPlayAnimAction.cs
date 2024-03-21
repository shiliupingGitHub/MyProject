using Game.Script.Attribute;
using UnityEngine;

namespace Game.Script.Character.Skill.Action
{
    [SkillDes(SkillType.PlayAnimation, "播放动作")]
    public class SkillPlayAnimSkillAction : SkillAction
    {
        [SerializeField]
        [Label("动作名")]
        public string aniName;

        [Label("测试float")] public float testFloat;
        [Label("测试int")] public float testInt;

    }
}