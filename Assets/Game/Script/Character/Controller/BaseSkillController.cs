using System;
using System.Collections.Generic;
using Game.Script.Game;
using UnityEngine;

namespace Game.Script.Character
{
    public class BaseSkillController : BaseController
    {
        public List<global::Skill.Skill> skills = new();

        private List<global::Skill.Skill> instanceSkills = new();
        private global::Skill.Skill curSkill = null;
        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            if (null != skills)
            {
                foreach (var skill in skills)
                {
                    var instanceSkill = Instantiate<global::Skill.Skill>(skill);
                    instanceSkills.Add(instanceSkill);
                    instanceSkill.Init();
                    
                }
            }
        }

        public override void Tick(float deltaTime)
        {
            base.Tick(deltaTime);

            if (null != curSkill)
            {
                curSkill.ExecuteSkill(deltaTime, this);
            }
            
        }
    }
}