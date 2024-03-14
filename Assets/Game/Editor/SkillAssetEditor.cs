using System.Linq;
using Game.Script.Character.Skill;
using UnityEditor;
using UnityEngine;

namespace Skill.Editor
{
    [CustomEditor(typeof(Skill))]
    public class SkillAssetEditor : UnityEditor.Editor
    {
        private Skill _skill = null;
        private float curTime = 0;
        private int selectActionIndex = 0;
        private void OnEnable()
        {
            _skill = target as Skill;
        }

       

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("持续时间", GUILayout.Width(100));
            _skill.maxTime = EditorGUILayout.FloatField( _skill.maxTime, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(50);
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();
          
            curTime = GUILayout.HorizontalSlider(curTime, 0, _skill.maxTime);
            curTime = EditorGUILayout.FloatField(curTime, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            var sortDes = SkillMgr.Instance.GetSortDes();
            GUILayout.Label("行为:");
            selectActionIndex = EditorGUILayout.Popup(selectActionIndex, sortDes.ToArray(), GUILayout.Width(100));
            if (GUILayout.Button("添加行为", GUILayout.Width(100)))
            {
                var type = (SkillType)selectActionIndex;
                if (SkillMgr.Instance.DefaultActions.TryGetValue(type, out var defaultAction))
                {
                     var param = defaultAction.GetDefaultParam();
                    _skill.actions.Add(new SkillActonConfig()
                    {
                        param = param, skillType = type, time = curTime,
                    });
                }
               
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.EndVertical();
        
    
            EditorGUILayout.Space(50);

            EditorGUILayout.BeginVertical();
            bool bRemove = false;
            SkillActonConfig removeActionConfig = null;
          
            foreach (var action in _skill.actions)
            {
                EditorGUILayout.BeginHorizontal();
                int skillTypeIndex = (int)action.skillType;

                if (sortDes.Count() > skillTypeIndex)
                {
                    
                    action.skillType = (SkillType)EditorGUILayout.Popup(skillTypeIndex, sortDes.ToArray(), GUILayout.Width(100));
                    action.time = EditorGUILayout.FloatField(action.time , GUILayout.Width(50));
                    EditorGUILayout.Space(50);
                    var oldColor = GUI.color;
                    GUI.color = Color.green;
                    EditorGUILayout.BeginVertical();
                    action.param = SkillMgr.Instance.DefaultActions[action.skillType].OnGui(action.param);
                    EditorGUILayout.EndVertical();
                    GUI.color = oldColor;
                    if (GUILayout.Button("X", GUILayout.Width(100)))
                    {
                        bRemove = true;
                        removeActionConfig = action;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            if (bRemove)
            {
                _skill.actions.Remove(removeActionConfig);
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