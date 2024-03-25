using System.Collections.Generic;
using UnityEngine;

namespace Game.Script.Map
{
    public class MapArea
    {
        public bool MapBlocked { get; set; } = false;
        private List<GameObject> _gameObjects = new();

        public void Enter(GameObject go)
        {
            if (!_gameObjects.Contains(go))
            {
                _gameObjects.Add(go);
            }
        }

        public bool Blocked
        {
            get
            {
                if (MapBlocked)
                {
                    return false;
                }
                return ActorBlocked;
            }
        }

        bool ActorBlocked => false;


        public void Leave(GameObject go)
        {
            _gameObjects.Remove(go);
        }
    }
}