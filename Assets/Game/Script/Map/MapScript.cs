using System;
using System.Collections.Generic;
using UnityEngine;
using Game.Script.Attribute;

namespace Game.Script.Map
{
    public class MapScript : MonoBehaviour
    {
        [Label("原点起始偏移")] public Vector3 originOffset;
        
        [Label("a*格子尺寸")] public float aStarSize = 1;

        [Label("a* X方向数量")]public int xAStarNum = 1;

        [Label("a* Y方向数量")]public int yAStarNum = 1;
        [SerializeField] public List<uint> blocks  = new();

        public bool IsBlock(uint x, uint y)
        {
            var a = x << 16;
            var index = a | y;

            return blocks.Contains(index);

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