using System.Collections.Generic;
using UnityEngine;

namespace Game.Script.Character
{
    public class BaseSkillController : BaseController
    {
        public List<global::Skill.Skill> skills = new();

        private List<global::Skill.Skill> instanceSkills = new();
        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            if (null != skills)
            {
                foreach (var skill in skills)
                {
                    var instanceSkill = Instantiate<global::Skill.Skill>(skill);
                    instanceSkills.Add(instanceSkill);
                }
            }
        }
    }
}