using System;
using System.Collections.Generic;
using System.Linq;
using Game.Script.Map;
using Mirror;
using UnityEngine;

namespace Game.Script.Common
{
    public class Actor : NetworkBehaviour
    {
        public virtual Vector2Int[] Areas => null;
        public virtual bool IsBlock => false;
        private Transform cacheTransform;
        private int areaIndex = -1;
        private List<(int, int)> nowArea = new();
        private List<(int, int)> tempArea = new();
        protected System.Action positionChanged;
        protected virtual void Start()
        {
            cacheTransform = transform;
            UpdateArea();
            positionChanged += UpdateArea;
        }

        protected virtual void OnDestroy()
        {
           LeaveAllArea();
        }

        void LeaveAllArea()
        {
            foreach (var area in nowArea)
            {
                var mapArea = MapMgr.Instance.GetArea(area.Item1, area.Item2);

                if (mapArea != null)
                {
                    mapArea.Leave(this, IsBlock);
                }
            }
            nowArea.Clear();
        }

        public virtual void UpdateArea()
        {
            
            var position = cacheTransform.position;
            (var nowAreaIndex, var x, var y) =  MapMgr.Instance.CreateAreaIndex(position);

            if (nowAreaIndex != areaIndex)
            {
                areaIndex = nowAreaIndex;

                if (areaIndex >= 0)
                {
                    tempArea.Clear();
                    if (Areas != null)
                    {
                        foreach (var block in Areas)
                        {
                            int gridX = x + block.x;
                            int gridY = y + block.y;
                            tempArea.Add((gridX, gridY));
                        }
                    }
                    else
                    {
                        tempArea.Add((x, y));
                    }

                    foreach (var area in tempArea)
                    {
                        if (!nowArea.Contains(area))
                        {
                            var mapArea = MapMgr.Instance.GetArea(area.Item1, area.Item2, true);
                            mapArea.Enter(this, IsBlock);
                        }
                    }

                    foreach (var area in nowArea)
                    {
                        if (!tempArea.Contains(area))
                        {
                            var mapArea = MapMgr.Instance.GetArea(area.Item1, area.Item2);

                            if (mapArea != null)
                            {
                                mapArea.Leave(this, IsBlock);
                            }
                        }
                    }
                    nowArea.Clear();
                    tempArea.CopyTo(nowArea);
                }
                else
                {
                    LeaveAllArea();
                }
            }
        }
    }
}