using System;
using System.Collections.Generic;
using System.Linq;
using Game.Script.Map;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Script.Common
{
    public enum ActorType
    {
        Normal,
        Shadow,
        Preview,
    }
    public class Actor : NetworkBehaviour
    {
        public virtual Vector2Int[] Areas => null;
        public virtual bool IsBlock => false;
        private Transform cacheTransform;
        private int areaIndex = -1;
        private List<(int, int)> nowArea = new();
        private List<(int, int)> tempArea = new();
        protected System.Action positionChanged;
        public Vector3 centerOffset = new Vector3(0.5f, 0.5f, 0);
        public virtual Vector3 CenterOffset => centerOffset;

        public ActorType ActorType { get; set; } = ActorType.Normal;
        
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
            var mapSubsystem = Game.Game.Instance.GetSubsystem<MapSubsystem>();
            foreach (var area in nowArea)
            {
                var mapArea = mapSubsystem.GetArea(area.Item1, area.Item2);

                if (mapArea != null)
                {
                    mapArea.Leave(this, IsBlock);
                }
            }
            nowArea.Clear();
        }

        public virtual void UpdateArea()
        {
            var mapSubsystem = Game.Game.Instance.GetSubsystem<MapSubsystem>();
            var position = cacheTransform.position;
            (var nowAreaIndex, var x, var y) =  mapSubsystem.CreateAreaIndex(position);

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
                            var mapArea = mapSubsystem.GetArea(area.Item1, area.Item2, true);
                            mapArea.Enter(this, IsBlock);
                        }
                    }

                    foreach (var area in nowArea)
                    {
                        if (!tempArea.Contains(area))
                        {
                            var mapArea = mapSubsystem.GetArea(area.Item1, area.Item2);

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