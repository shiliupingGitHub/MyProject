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
    [CustomEditor(typeof(MapScript))]
    public class MapScriptEditor : UnityEditor.Editor
    {
        private bool bAddedViewCallback = false;
        private MapScript _mapScript;
      
        void AddViewCallBack()
        {
            if (_mapScript.gameObject.scene.isLoaded)
            {
                if (!bAddedViewCallback)
                {
                    SceneView.duringSceneGui += OnSceneView;
                    bAddedViewCallback = true;
                }
            }
         
        }

        void RemoveViewCallBack()
        {
            if (bAddedViewCallback)
            {
                SceneView.duringSceneGui -= OnSceneView;
                bAddedViewCallback = false;
            }
        }

        private void Awake()
        {
            _mapScript = target as MapScript;
            AddViewCallBack();
        }

        private void OnEnable()
        {
            _mapScript = target as MapScript;
            
            _mapScript.StartEdit();
        }

        private void OnDestroy()
        {
            RemoveViewCallBack();
        }
        
        void SetBlock(MapScript mapScript, int x, int y, bool block)
        {
            
            mapScript.SetBlock((uint)x, (uint)y, block);
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }
        

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            bool isEdit = EditorGUILayout.Toggle("编辑阻挡", _mapScript.EnableEdit);

            if (isEdit != _mapScript.EnableEdit)
            {
                _mapScript.EnableEdit = isEdit;
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
            }
            
        }
        
        void OnSceneView(SceneView sceneView)
        {
            
            var mapScript = _mapScript;
            if (mapScript)
            {
                if (mapScript.EnableEdit)
                {
                    mapScript.StartEdit();
                    sceneView.autoRepaintOnSceneChange = true;
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