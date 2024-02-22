using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    [CustomEditor(typeof(SkillAsset))]
    public class SkillAssetEditor : UnityEditor.Editor
    {
        private SkillAsset _skillAsset = null;
        private void OnEnable()
        {
            _skillAsset = target as SkillAsset;
        }
    }
}