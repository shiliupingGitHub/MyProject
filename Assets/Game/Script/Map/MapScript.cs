using System;
using UnityEngine;
using Game.Script.Attribute;

namespace Game.Script.Map
{
    public class MapScript : MonoBehaviour
    {
        [Label("原点起始偏移")] public Vector3 originOffset;
        
        [Label("a*格子尺寸")] public float aStarSize;

        [Label("a* X方向数量")]public int xAStarNum;

        [Label("a* Y方向数量")]public int yAStarNum;
    }
}