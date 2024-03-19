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
        
        private Mesh _blockMesh;
        private Mesh _bkMesh;
        private Material _blockMat;
        private Material _bkMat;
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        public bool IsBlock(uint x, uint y)
        {
            var a = x << 16;
            var index = a | y;

            return blocks.Contains(index);

        }


        public bool ShowGrid { get; set; } = false;
        public  void StartEdit()
        {
            if (_blockMesh == null)
            {
                _blockMesh = new Mesh();
            }

            if (_bkMesh == null)
            {
                _bkMesh = new Mesh();
            }
            
            if (_blockMat == null)
            {
                _blockMat = new Material(Shader.Find("Shader Graphs/DrawAStar"));
                _blockMat.enableInstancing = true;
            }
            
            if (_bkMat == null)
            {
                _bkMat = new Material(Shader.Find("Shader Graphs/DrawAStar"));
                _bkMat.enableInstancing = true;
            }

            CreateBkMesh();
            CreateBlockMesh();
        }

        public void StopEdit()
        {
            _blockMesh = null;
            _bkMesh = null;
            _blockMat = null;
            _bkMat = null;

        }
        
        void CreateBkMesh()
        {
            _bkMesh.Clear();
            
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(gridSize * xGridNum , 0, 0),
                new Vector3(0, gridSize * yGridNum, 0),
                new Vector3(gridSize * xGridNum, gridSize * yGridNum , 0)
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
        
        void  CreateBlockMesh()
        {
       
            _blockMesh.Clear();
            
            Vector3[] vertices = new Vector3[4]
            {
                new Vector3(0, 0, 0),
                new Vector3(gridSize , 0, 0),
                new Vector3(0, gridSize, 0),
                new Vector3(gridSize, gridSize , 0)
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

        public (uint, uint) DeCodeIndex(uint index)
        {
            uint retX = 0;
            uint retY = 0;

            retX = index >> 16;

            retY = index << 16 >> 16;

            return (retX, retY);
        }

        public void DrawGrid()
        {
            if (_bkMesh != null && _bkMat != null)
            {
                List<Matrix4x4> bkMatrixs = new();
                Vector3 bkPosition = transform.position;
                bkPosition += originOffset;

                bkPosition += new Vector3(0, 0, -0.5f);

                var bkMatrix = Matrix4x4.TRS(bkPosition, quaternion.identity, Vector3.one);
                bkMatrixs.Add(bkMatrix);
                _bkMat.enableInstancing = true;
                _bkMat.SetColor(Color1, Color.green);
                Graphics.DrawMeshInstanced(_bkMesh, 0, _bkMat, bkMatrixs.ToArray(), bkMatrixs.Count);
            }
            
            List<Matrix4x4> matrix4X4s = new();
            Vector3 offset = transform.position + originOffset;
            foreach (var block in blocks)
            {
                (uint x, uint y) = DeCodeIndex(block);

                Vector3 position = new Vector3(x * gridSize, y * gridSize, -1);

                position += offset;
                Matrix4x4 matrix4 =  Matrix4x4.TRS(position, quaternion.identity, Vector3.one);
                     
                matrix4X4s.Add(matrix4);
                     
            }
            
            if (matrix4X4s.Count > 0 && _blockMat != null && _blockMesh != null)
            {
                _blockMat.SetColor(Color1, Color.red);
                _blockMat.enableInstancing = true;
                Graphics.DrawMeshInstanced(_blockMesh, 0, _blockMat, matrix4X4s.ToArray(), matrix4X4s.Count);
            }
        }
        
        private void Update()
        {
            if (ShowGrid)
            {
                DrawGrid();
            }
        }

        public Vector3 GetGridStartPosition(int x, int y)
        {
            Vector3 o = transform.position + originOffset;

            o.x += x * gridSize;
            o.y += y * gridSize;
            o.z = -1;
            return o;
        }
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