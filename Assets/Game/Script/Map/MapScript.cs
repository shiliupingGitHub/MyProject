using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Script.Attribute;
using Unity.Mathematics;
using UnityEngine.Serialization;

namespace Game.Script.Map
{
    [ExecuteInEditMode]
    public class MapScript : MonoBehaviour
    {
        [Label("原点起始偏移")] public Vector3 originOffset;
        
        [Label("格子尺寸")] public float gridSize = 1;

        [Label("X方向数量")]public int xGridNum = 100;

        [Label("Y方向数量")]public int yGridNum = 100;
        [SerializeField] public List<uint> blocks  = new();
        
        private Mesh _gridMesh;
        public Material blockMat;
        public Material gridMat;
        private static readonly int Color1 = Shader.PropertyToID("_Color");
        [NonSerialized] public bool EnableEdit = false;

        public bool IsBlock(uint x, uint y)
        {
            var a = x << 16;
            var index = a | y;

            return blocks.Contains(index);

        }


        public bool ShowGrid { get; set; } = false;
        public bool ShowBlock { get; set; } = false;
        public  void StartEdit()
        {
            if (_gridMesh == null)
            {
                _gridMesh = new Mesh();
            }
            
            CreateGrodMesh();
        }

        public void StopEdit()
        {
            _gridMesh = null;
            blockMat = null;
            gridMat = null;

        }
        
        void  CreateGrodMesh()
        {
       
            _gridMesh.Clear();
            
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(gridSize , 0, 0),
                new Vector3(0, gridSize, 0),
                new Vector3(gridSize, gridSize , 0)
            };
                
            _gridMesh.vertices = vertices;

            int[] tris = new int[6]
            {
                // lower left triangle
                0, 2, 1,
                // upper right triangle
                2, 3, 1
            };
            _gridMesh.triangles = tris;
            
            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            _gridMesh.uv = uv;
            
            
        }

        public (uint, uint) DeCodeIndex(uint index)
        {
            uint retX = 0;
            uint retY = 0;

            retX = index >> 16;

            retY = index << 16 >> 16;

            return (retX, retY);
        }
        List<Matrix4x4> griMatrix4X4s = new();
        private int oldGridX = 0;
        private int oldGridY = 0;
        void DrawGrid()
        {
            if (_gridMesh != null && gridMat != null)
            {

                if (oldGridX != xGridNum || oldGridY != yGridNum)
                {
                    griMatrix4X4s.Clear();
                    oldGridX = xGridNum;
                    oldGridY = yGridNum;
                    Vector3 offset = transform.position + originOffset;
                    for (int x = 0; x < xGridNum; x++)
                    {
                        for (int y = 0; y < yGridNum; y++)
                        {
                            Vector3 position = new Vector3(x * gridSize, y * gridSize, -1);

                            position += offset;
                            Matrix4x4 matrix4 =  Matrix4x4.TRS(position, quaternion.identity, Vector3.one);
                     
                            griMatrix4X4s.Add(matrix4);
                        }
                    }
                }
                
                if (griMatrix4X4s.Count > 0 )
                {
                    Graphics.DrawMeshInstanced(_gridMesh, 0, gridMat, griMatrix4X4s.ToArray(), griMatrix4X4s.Count);
                }
            }
        }

        void DrawBlock()
        {
            List<Matrix4x4> matrix4X4s = new();
            Vector3 offset = transform.position + originOffset;
            foreach (var block in blocks)
            {
                (uint x, uint y) = DeCodeIndex(block);

                Vector3 position = new Vector3(x * gridSize, y * gridSize, -10);

                position += offset;
                Matrix4x4 matrix4 =  Matrix4x4.TRS(position, quaternion.identity, Vector3.one);
                     
                matrix4X4s.Add(matrix4);
                     
            }
            if (matrix4X4s.Count > 0 && blockMat != null && _gridMesh != null)
            {
                Graphics.DrawMeshInstanced(_gridMesh, 0, blockMat, matrix4X4s.ToArray(), matrix4X4s.Count);
            }
        }

        public void Draw()
        {
            if (ShowGrid)
            {
               DrawGrid();
            }

            if (ShowBlock)
            {
                DrawBlock();
            }
 
        }
        
        private void Update()
        {
            Draw();
        }

        public Vector3 GetGridStartPosition(int x, int y)
        {
            Vector3 o = transform.position + originOffset;

            o.x += x * gridSize;
            o.y += y * gridSize;
            o.z = -1;
            return o;
        }

        public Vector3 Offset => transform.position + originOffset;

        public (int, int) GetGridIndex(Vector3 worldPos)
        {

            int retX = -1;
            int retY = -1;

            Vector3 o = transform.position + originOffset;
            Vector3 max = o + new Vector3(gridSize * xGridNum, gridSize * yGridNum);

            var offset = (worldPos - o) / gridSize;

            if (offset.x >= 0 && offset.x < xGridNum)
            {
                retX = Mathf.FloorToInt(offset.x);
            }
            
            if (offset.y >= 0 && offset.y < yGridNum)
            {
                retY = Mathf.FloorToInt(offset.y);
            }
            return (retX, retY);
        }
        public void SetBlock(uint x, uint y, bool block)
        {
            var a = x << 16;
            var index = a | y;

            if (block)
            {
                if (!blocks.Contains(index))
                {
                    blocks.Add(index);
                }
            }
            else
            {
                if (blocks.Contains(index))
                {
                    blocks.Remove(index);
                }
            }
        }
    }
}