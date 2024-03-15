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

        private Mesh _blockMesh;
        private Mesh _bkMesh;
        private Material _blockMaterial;
        private Material _bkMaterial;
        

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
            if (_blockMesh == null)
            {
                _blockMesh = new Mesh();
            }

            if (_bkMesh == null)
            {
                _bkMesh = new Mesh();
            }
            
            if (_blockMaterial == null)
            {
                _blockMaterial = new Material(Shader.Find("Shader Graphs/DrawAStar"));
                _blockMaterial.enableInstancing = true;
            }
            
            if (_bkMaterial == null)
            {
                _bkMaterial = new Material(Shader.Find("Shader Graphs/DrawAStar"));
                _bkMaterial.enableInstancing = true;
            }
        }

        private void OnBecameInvisible()
        {
            var mapScript = GameObject.FindObjectOfType<MapScript>();
            if (mapScript != null)
            {
                mapScript.SetDraw(null, null, null, null, new List<Matrix4x4>());
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

        void CreateBkMesh(MapScript mapScript)
        {
            _bkMesh.Clear();
            
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(mapScript.aStarSize * mapScript.xAStarNum , 0, 0),
                new Vector3(0, mapScript.aStarSize * mapScript.yAStarNum, 0),
                new Vector3(mapScript.aStarSize * mapScript.xAStarNum, mapScript.aStarSize * mapScript.yAStarNum , 0)
            };
                
            _bkMesh.vertices = vertices;

            int[] tris = new int[6]
            {
                // lower left triangle
                0, 2, 1,
                // upper right triangle
                2, 3, 1
            };
            _bkMesh.triangles = tris;
            
            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            _bkMesh.uv = uv;
        }
        
        void  CreateBlockMesh(MapScript mapScript)
        {
       
            _blockMesh.Clear();
            
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(mapScript.aStarSize , 0, 0),
                new Vector3(0, mapScript.aStarSize, 0),
                new Vector3(mapScript.aStarSize, mapScript.aStarSize , 0)
            };
                
            _blockMesh.vertices = vertices;

            int[] tris = new int[6]
            {
                // lower left triangle
                0, 2, 1,
                // upper right triangle
                2, 3, 1
            };
            _blockMesh.triangles = tris;
            
            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            _blockMesh.uv = uv;
            
            
        }

        void FillMesh(Camera camera)
        {
            
            var mapScript = GameObject.FindObjectOfType<MapScript>();
            
            if (mapScript != null)
            {
                 CreateBlockMesh(mapScript);
                 CreateBkMesh(mapScript);
                 List<Matrix4x4> matrix4X4s = new();
                 Vector3 offset = mapScript.transform.position + mapScript.originOffset;
                 foreach (var block in mapScript.blocks)
                 {
                     (uint x, uint y) = mapScript.DeCodeIndex(block);

                     Vector3 position = new Vector3(x * mapScript.aStarSize, y * mapScript.aStarSize, -1);

                     position += offset;
                     Matrix4x4 matrix4 =  Matrix4x4.TRS(position, quaternion.identity, Vector3.one);
                     
                     matrix4X4s.Add(matrix4);
                     
                 }
                 mapScript.SetDraw(_blockMesh, _bkMesh, _blockMaterial, _bkMaterial, matrix4X4s);
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
            if (mapScript)
            {
                if (bEnableEdit)
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
                else
                {
                    mapScript.SetDraw(null, null, null, null, new List<Matrix4x4>());
                    
                }
            }
           
        }
    }
}