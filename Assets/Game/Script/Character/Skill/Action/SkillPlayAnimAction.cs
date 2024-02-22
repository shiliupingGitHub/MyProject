using UnityEngine;

namespace Game.Script.Character.Skill.Action
{
    [UnityEngine.CreateAssetMenu(fileName = "Action", menuName = "技能行为/技能释放动作", order = 0)]
    public class SkillPlayAnimAction : global::Skill.Action
    {
        [SerializeField] private string tag;
    }
}