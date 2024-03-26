using System.Collections.Generic;
using Game.Script.Common;
using UnityEngine;

namespace Game.Script.Map
{
    public class MapArea
    {
        public bool MapBlocked { get; set; } = false;
        private List<Actor> _actors= new();
        private List<Actor> _blockActors = new();

        public void Enter(Actor actor, bool block)
        {
            if (!_actors.Contains(actor))
            {
                _actors.Add(actor);
            }

            if (block)
            {
                if (!_blockActors.Contains(actor))
                {
                    _blockActors.Add(actor);
                }
            }
        }
        
        public void Leave(Actor actor, bool block)
        {
            _actors.Remove(actor);
            if (block)
            {
                _blockActors.Remove(actor);
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

        bool ActorBlocked
        {
            get
            {
                return _blockActors.Count > 0;
            }
        }
        
    }
}