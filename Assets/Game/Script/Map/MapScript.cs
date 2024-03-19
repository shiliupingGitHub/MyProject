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

        public bool IsBlock(uint x, uint y)
        {
            var a = x << 16;
            var index = a | y;

            return blocks.Contains(index);

        }

        public (uint, uint) DeCodeIndex(uint index)
        {
            uint retX = 0;
            uint retY = 0;

            retX = index >> 16;

            retY = index << 16 >> 16;

            return (retX, retY);
        }
        private Mesh _blockMesh;
        private Mesh _bkMesh;
        private Material _blockMat;
        private Material _bkMat;
        private List<Matrix4x4> _blockMatrix4X4s;
        private static readonly int Color1 = Shader.PropertyToID("_Color");

        public void SetDraw(Mesh mesh, Mesh bkMesh, Material blockMat, Material bkMat,List<Matrix4x4> matrix4X4s)
        {
            _blockMesh = mesh;
            _blockMat = blockMat;
            _bkMesh = bkMesh;
            _bkMat = bkMat;
            _blockMatrix4X4s = matrix4X4s;
        }
        private void Update()
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
            if (_blockMatrix4X4s != null && _blockMatrix4X4s.Count > 0 && _blockMat != null && _blockMesh != null)
            {
                _blockMat.SetColor(Color1, Color.red);
                _blockMat.enableInstancing = true;
                Graphics.DrawMeshInstanced(_blockMesh, 0, _blockMat, _blockMatrix4X4s.ToArray(), _blockMatrix4X4s.Count);
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