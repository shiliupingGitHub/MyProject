using System.Linq;
using Game.Script.Character.Skill;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    [CustomEditor(typeof(SkillAsset))]
    public class SkillAssetEditor : UnityEditor.Editor
    {
        private SkillAsset _skillAsset = null;
        private float curTime = 0;
        private int selectActionIndex = 0;
        private void OnEnable()
        {
            _skillAsset = target as SkillAsset;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            _skillAsset.maxTime = EditorGUILayout.FloatField("持续时间", _skillAsset.maxTime);
            
            EditorGUILayout.Space(50);
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();
          
            curTime = GUILayout.HorizontalSlider(curTime, 0, _skillAsset.maxTime);
            curTime = EditorGUILayout.FloatField(curTime);
            EditorGUILayout.EndHorizontal();

            var sortDes = SkillMgr.Instance.GetSortDes();
            selectActionIndex = EditorGUILayout.Popup("行为", selectActionIndex, sortDes.ToArray());
            if (GUILayout.Button("添加行为"))
            {
                var type = (SkillType)selectActionIndex;
                if (SkillMgr.Instance.DefaultActions.TryGetValue(type, out var defaultAction))
                {
                     var param = defaultAction.GetDefaultParam();
                    _skillAsset.cacheActions.Add(new SkillActonConfig()
                    {
                        actionParam = param, skillType = type, time = curTime,
                    });
                }
               
            }
            EditorGUILayout.EndVertical();
        
    
            EditorGUILayout.Space(50);

            EditorGUILayout.BeginVertical();
            bool bRemove = false;
            SkillActonConfig removeActionConfig = null;
            foreach (var action in _skillAsset.cacheActions)
            {
                EditorGUILayout.BeginHorizontal();
                int skillTypeIndex = (int)action.skillType;

                if (sortDes.Count() > skillTypeIndex)
                {
                    var des = sortDes[skillTypeIndex];
                    
                    GUILayout.Label(des);
                    action.time = EditorGUILayout.FloatField(action.time);
                    action.actionParam = SkillMgr.Instance.DefaultActions[action.skillType].OnGui(action.actionParam);
                    if (GUILayout.Button("X"))
                    {
                        bRemove = true;
                        removeActionConfig = action;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            if (bRemove)
            {
                _skillAsset.cacheActions.Remove(removeActionConfig);
            }
            EditorGUILayout.EndVertical();
         
          
            
            EditorGUILayout.EndVertical();
            //base.OnInspectorGUI();
            // serializedObject.Update();
            // serializedObject.FindProperty("").s
            // serializedObject.ApplyModifiedProperties();
        }
    }
}