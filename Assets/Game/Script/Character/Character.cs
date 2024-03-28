
using System.Collections.Generic;
using UnityEngine;

namespace Game.Script.Character
{
    public class Character : Pawn
    {
        public List<global::Skill.Skill> skills = new();

        private List<global::Skill.Skill> instanceSkills = new();
        private global::Skill.Skill curSkill = null;

        private Vector3 _lastPosition = new Vector3(-1000, -1000, 0);
        private Transform _cacheTransform;
        
        public override void Awake()
        {
            base.Awake();
            _cacheTransform = transform;
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

            if (_lastPosition != _cacheTransform.position)
            {
                _lastPosition = _cacheTransform.position;
                positionChanged?.Invoke();
            }
            
        }
    }
}