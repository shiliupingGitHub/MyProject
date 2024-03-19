using System;
using System.Collections.Generic;
using Game.Script.Map;
using NUnit.Framework;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

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
            window.minSize = new Vector2(500, 500);
            window.Show();
        }
        
        private void OnBecameVisible()
        {
            var mapScript = GameObject.FindObjectOfType<MapScript>();
            if (null != mapScript)
            {
                mapScript.StartEdit();
            }
        }

        private void OnBecameInvisible()
        {
            var mapScript = GameObject.FindObjectOfType<MapScript>();
            if (mapScript != null)
            {
                mapScript.StartEdit();
            }
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
        
        void SetBlock(MapScript mapScript, int x, int y, bool block)
        {
            
            mapScript.SetBlock((uint)x, (uint)y, block);
        }



        void OnSceneView(SceneView sceneView)
        {
            var mapScript = GameObject.FindObjectOfType<MapScript>();
            if (mapScript)
            {
                if (bEnableEdit)
                {
                    HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

                    mapScript.ShowGrid = true;
                    mapScript.ShowBlock = true;
                    if (Event.current.isMouse && Event.current.button == 0 && Event.current.type == EventType.MouseDown)
                    {
                        var mousePos = Event.current.mousePosition;
                        mousePos.y = Camera.current.pixelHeight - mousePos.y;
                        var worldPos =  sceneView.camera.ScreenToWorldPoint(mousePos);

                        (int x, int y) = mapScript.GetGridIndex(worldPos);

                        if (x >= 0 && y >= 0)
                        {

                            if (Event.current.shift)
                            {
                                SetBlock(mapScript, x, y, false);
                            }
                            else
                            {
                                SetBlock(mapScript, x, y, true);
                            }
                        }
                    }
                }
                else
                {
                    mapScript.ShowGrid = false;
                    mapScript.ShowBlock = false;

                }
            }
           
        }
    }
}