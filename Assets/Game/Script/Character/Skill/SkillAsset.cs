using System;
using System.Collections.Generic;
using Game.Script.Character.Skill;
using UnityEngine;

namespace Skill
{
    [Serializable]
    public class SkillActonConfig
    {
        [SerializeField] public float time = 0;
        [SerializeField] public SkillType skillType;
        [SerializeField] public string actionParam;
    }
    
    [CreateAssetMenu(fileName = "skill", menuName = "技能配置文件", order = 0)]
    public class SkillAsset : ScriptableObject
    {
        [SerializeField] public float maxTime = 1;
        [SerializeField] public List<SkillActonConfig> cacheActions;
    }
}