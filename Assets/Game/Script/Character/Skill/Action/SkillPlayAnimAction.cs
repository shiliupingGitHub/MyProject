using UnityEngine;

namespace Game.Script.Character.Skill.Action
{
    [SkillDes(SkillType.PlayAnimation, "播放动作")]
    public class SkillPlayAnimSkillAction : SkillAction
    {
        private string aniName;
        public override void ParseParam(string param)
        {
            base.ParseParam(param);
            aniName = param;
        }

        public override string OnGui(string param)
        {
            base.OnGui(param);
            string ret = param;
            ParseParam(param);
#if UNITY_EDITOR
            UnityEngine.GUILayout.Label("动作");
            aniName = UnityEngine.GUILayout.TextField(aniName);
            ret = aniName;
#endif
            return ret;
        }
    }
}