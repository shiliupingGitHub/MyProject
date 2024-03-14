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
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("持续时间", GUILayout.Width(100));
            _skillAsset.maxTime = EditorGUILayout.FloatField( _skillAsset.maxTime, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(50);
            EditorGUILayout.BeginVertical();
            
            EditorGUILayout.BeginHorizontal();
          
            curTime = GUILayout.HorizontalSlider(curTime, 0, _skillAsset.maxTime);
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
                    _skillAsset.cacheActions.Add(new SkillActonConfig()
                    {
                        actionParam = param, skillType = type, time = curTime,
                    });
                }
               
            }
            EditorGUILayout.EndHorizontal();
            
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
                    
                    action.skillType = (SkillType)EditorGUILayout.Popup(skillTypeIndex, sortDes.ToArray(), GUILayout.Width(100));
                    action.time = EditorGUILayout.FloatField(action.time , GUILayout.Width(50));
                    EditorGUILayout.Space(50);
                    var oldColor = GUI.color;
                    GUI.color = Color.green;
                    EditorGUILayout.BeginVertical();
                    action.actionParam = SkillMgr.Instance.DefaultActions[action.skillType].OnGui(action.actionParam);
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