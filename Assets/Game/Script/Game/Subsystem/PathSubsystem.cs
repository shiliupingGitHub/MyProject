﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityParticleSystem;
using Game.Script.Level;
using Game.Script.Map;
using Game.Script.Misc;
using Priority_Queue;
using UnityEngine;

namespace Game.Script.Game.Subsystem
{
    struct PathRequest
    {
        public Vector3 startPosition;
        public Vector3 endPosition;
        public ulong pathId;
    }

    public class PathSubsystem : GameSubsystem
    {
        private readonly Dictionary<ulong, List<(int, int)>> _pathResponses = new();
        private readonly List<PathRequest> _pathRequestList = new();
        private ulong _pathId = 1;
        private const int PathNumPerFrame = 20;
        private MapScript _mapScript;


        public MapScript MapScript
        {
            set => _mapScript = value;
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            LevelManager.Instance.preLevelChange += (type, levelType) =>
            {
                _pathResponses.Clear();
                _pathRequestList.Clear();
                _pathId = 1;
            };
            MapMgr.Instance.loadedMap += go =>
            {
                if (null != go)
                {
                    _mapScript = go.GetComponent<MapScript>();
                }
            };
            GameTickManager.Instance.AddTick(OnTick);
        }

        ulong AddPath(Vector3 start, Vector3 end)
        {
            var curPathId = _pathId;
            _pathId++;

            _pathRequestList.Add(new PathRequest() { startPosition = start, endPosition = end, pathId = curPathId });
            return _pathId++;
        }

        void RemovePath(ulong pathId)
        {
            if (_pathResponses.ContainsKey(pathId))
            {
                _pathResponses.Remove(pathId);
            }
            else
            {
                _pathRequestList.RemoveAll(x => x.pathId == pathId);
            }
        }

        void OnTick()
        {
            if (_mapScript == null)
            {
                return;
            }

            int Num = Mathf.Min(PathNumPerFrame, _pathRequestList.Count);

            if (Num > 0)
            {
                Parallel.For(0, Num - 1, (i, state) =>
                {
                    var request = _pathRequestList[i];

                    var path = DoPath(request.startPosition, request.endPosition, _mapScript);

                    lock (this)
                    {
                        _pathResponses.Add(request.pathId, path);
                    }
                });
                _pathRequestList.RemoveRange(0, Num);
            }
        }

        List<(int, int)> DoPath(Vector3 start, Vector3 end, MapScript mapScript)
        {
            (int startX, int startY) = mapScript.GetGridIndex(start);
            (int endX, int endY) = mapScript.GetGridIndex(end);
            return GeneratePath(startX, startY, endX, endY, mapScript);
        }

        private float calcHeuristicManhattan(int nodeX, int nodeY, int goalX, int goalY)
        {
            return Mathf.Abs(nodeX - goalX) + Mathf.Abs(nodeY - goalY);
        }

        bool IsBlock(MapScript walkableMap, int x, int y)
        {
            return walkableMap.IsBlock((uint)x, (uint)y);
        }

        // Calculates the Euclidean heuristic distance between two nodes
        private float calcHeuristicEuclidean(int nodeX, int nodeY, int goalX, int goalY)
        {
            return Mathf.Sqrt(Mathf.Pow(nodeX - goalX, 2) + Mathf.Pow(nodeY - goalY, 2));
        }

        private List<(int, int)> backtracePath((int, int)[,] parentMap, int goalX, int goalY)
        {
            List<(int, int)> path = new List<(int, int)>();

            (int, int) current = (goalX, goalY);

            while (current != (-1, -1))
            {
                path.Add(current);
                current = parentMap[current.Item2, current.Item1];
            }

            path.Reverse();
            return path;
        }

        private (bool, (int, int))[] getNeighbours(int xCordinate, int yCordinate, MapScript walkableMap, bool walkableDiagonals = false)
        {
            List<(bool, (int, int))> neighbourCells = new List<(bool, (int, int))>();

            int heigth = walkableMap.yGridNum;
            int width = walkableMap.xGridNum;

            int range = 1;
            int yStart = (int)MathF.Max(0, yCordinate - range);
            int yEnd = (int)MathF.Min(heigth - 1, yCordinate + range);

            int xStart = (int)MathF.Max(0, xCordinate - range);
            int xEnd = (int)MathF.Min(width - 1, xCordinate + range);

            for (int y = yStart; y <= yEnd; y++)
            {
                for (int x = xStart; x <= xEnd; x++)
                {
                    if (x == xCordinate && y == yCordinate)
                    {
                        continue;
                    }

                    if (!IsBlock(walkableMap, x, y))
                    {
                        continue;
                    }

                    if (!walkableDiagonals && // If we are not allowing diagonal movement
                        (x == xStart || x == xEnd) && (y == yStart || y == yEnd) && // If the node is a diagonal node
                        IsBlock(walkableMap, xCordinate, y) && IsBlock(walkableMap, x, yCordinate)) // If the node is not reachable from the current node
                    {
                        continue;
                    }

                    neighbourCells.Add((!IsBlock(walkableMap, x, y), (x, y)));
                }
            }

            return neighbourCells.ToArray();
        }


        public List<(int, int)> GeneratePath(int startX, int startY, int goalX, int goalY, MapScript walkableMap, bool manhattanHeuristic = true, bool walkableDiagonals = false)
        {
            // Set the heuristic function to use based on the manhattanHeuristic parameter
            Func<int, int, int, int, float> heuristic = manhattanHeuristic ? calcHeuristicManhattan : calcHeuristicEuclidean;

            // Get the dimensions of the map
            int mapHeight = walkableMap.yGridNum;
            int mapWidth = walkableMap.xGridNum;

            // Define constants and arrays needed for the algorithm
            float sqrt2 = Mathf.Sqrt(2);
            float[,] gCostMap = new float[mapHeight, mapWidth];
            float[,] fCostMap = new float[mapHeight, mapWidth];
            (int, int)[,] parentMap = new (int, int)[mapHeight, mapWidth];

            // Initialize the open set with the starting node
            SimplePriorityQueue<(int, int)> openSet = new SimplePriorityQueue<(int, int)>();
            gCostMap[startY, startX] = 0.0000001f;
            fCostMap[startY, startX] = heuristic(startX, startY, goalX, goalY);
            parentMap[startY, startX] = (-1, -1);
            openSet.Enqueue((startX, startY), fCostMap[startY, startX]);

            // Start the A* algorithm
            while (openSet.Count > 0)
            {
                // Get the node with the lowest f cost from the open set
                (int, int) current = openSet.First;
                int currentX = current.Item1;
                int currentY = current.Item2;

                // If we have reached the goal node, backtrack to get the path
                if (current.Item1 == goalX && current.Item2 == goalY)
                {
                    var path = backtracePath(parentMap, goalX, goalY);
                    return path;
                }

                // Remove the current node from the open set
                openSet.Dequeue();

                // Get the neighbours of the current node
                (bool, (int, int))[] neighbours = getNeighbours(currentX, currentY, walkableMap, walkableDiagonals);

                // Process each neighbour
                for (int i = 0; i < neighbours.Length; i++)
                {
                    (bool, (int, int)) neighbour = neighbours[i];

                    // Get the x and y coordinates of the neighbour
                    int neighbourX = neighbour.Item2.Item1;
                    int neighbourY = neighbour.Item2.Item2;

                    // Calculate the tentative g cost of the neighbour
                    float dist = currentX - neighbourX == 0 || currentY - neighbourY == 0 ? 1 : sqrt2;
                    float tentativeGCost = gCostMap[currentY, currentX] + dist;

                    // If the neighbour has not been visited yet, or the tentative g cost is lower than its current g cost,
                    // update its g, f and parent values and add it to the open set
                    if (gCostMap[neighbourY, neighbourX] == 0 || tentativeGCost < gCostMap[neighbourY, neighbourX])
                    {
                        parentMap[neighbourY, neighbourX] = current;
                        gCostMap[neighbourY, neighbourX] = tentativeGCost;
                        fCostMap[neighbourY, neighbourX] = tentativeGCost + heuristic(neighbourX, neighbourY, goalX, goalY);

                        // If the neighbor node is not in the open set, add it
                        if (!openSet.Contains(neighbour.Item2))
                        {
                            openSet.Enqueue(neighbour.Item2, fCostMap[neighbourY, neighbourX]);
                        }
                    }
                }
            }

            return null;
        }
    }
}