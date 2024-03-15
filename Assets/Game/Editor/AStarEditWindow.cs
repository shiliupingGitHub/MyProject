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

        private Mesh _mesh;
        private Material _material;
        

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
            if (_mesh == null)
            {
                _mesh = new Mesh();
            }
            
            if (_material == null)
            {
                _material = new Material(Shader.Find("Unlit/DrawAstarShader"));
            }
        }

        private void OnBecameInvisible()
        {
            
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
        
        void  CreateMesh(MapScript mapScript)
        {
       
            _mesh.Clear();
            
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(mapScript.aStarSize * mapScript.xAStarNum, 0, 0),
                new Vector3(0, mapScript.aStarSize * mapScript.yAStarNum, 0),
                new Vector3(mapScript.aStarSize * mapScript.xAStarNum, mapScript.aStarSize * mapScript.yAStarNum, 0)
            };
                
            _mesh.vertices = vertices;

            int[] tris = new int[6]
            {
                // lower left triangle
                0, 2, 1,
                // upper right triangle
                2, 3, 1
            };
            _mesh.triangles = tris;
            
            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            _mesh.uv = uv;
            
            
        }

        void FillMesh(Camera camera)
        {
            
            var mapScript = GameObject.FindObjectOfType<MapScript>();
            
            if (mapScript != null)
            {
                 CreateMesh(mapScript);
                 
            }
        }

        (int, int) GetGridIndex( MapScript mapScript, Vector3 worldPos)
        {

            int retX = -1;
            int retY = -1;

            Vector3 o = mapScript.transform.position + mapScript.originOffset;
            Vector3 max = o + new Vector3(mapScript.aStarSize * mapScript.xAStarNum, mapScript.aStarSize * mapScript.yAStarNum);

            var offset = (worldPos - o) / mapScript.aStarSize;

            if (offset.x >= 0 && offset.x < mapScript.xAStarNum)
            {
                retX = Mathf.FloorToInt(offset.x);
            }
            
            if (offset.y >= 0 && offset.y < mapScript.yAStarNum)
            {
                retY = Mathf.FloorToInt(offset.y);
            }
            return (retX, retY);
        }

        void SetBlock(MapScript mapScript, int x, int y, bool block)
        {
            
            mapScript.SetBlock((uint)x, (uint)y, block);
        }

        void OnSceneView(SceneView sceneView)
        {
            var mapScript = GameObject.FindObjectOfType<MapScript>();
            if (bEnableEdit && mapScript)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
                
               FillMesh(sceneView.camera);
               if (Event.current.isMouse && Event.current.button == 0 && Event.current.type == EventType.MouseDown)
               {
                   var mousePos = Event.current.mousePosition;
                   mousePos.y = Camera.current.pixelHeight - mousePos.y;
                   var worldPos =  sceneView.camera.ScreenToWorldPoint(mousePos);

                   (int x, int y) = GetGridIndex(mapScript, worldPos);

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
        }
    }
}