using System;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public class AStarEditWindow : EditorWindow
    {
        private bool bEnableEdit = false;
        [MenuItem("AStar/Edit")]
        private static void ShowWindow()
        {
            var window = GetWindow<AStarEditWindow>();
            window.titleContent = new GUIContent("AStar");
            window.Show();
        }
      
        private void OnGUI()
        {
            bEnableEdit = GUILayout.Toggle(bEnableEdit, "启用编辑");
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneView;
        }

        private void OnDestroy()
        {
            SceneView.duringSceneGui -= OnSceneView;
        }

        void OnSceneView(SceneView sceneView)
        {
            if (bEnableEdit)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                
               
            }
        }
    }
}