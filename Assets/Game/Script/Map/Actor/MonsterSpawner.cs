using System.Collections.Generic;
using Game.Script.Character;
using Game.Script.Common;
using Mirror;
using UnityEngine;

namespace Game.Script.Map.Actor
{
    public class MonsterSpawner : MapActor
    {
        public List<GameObject> spawns = new();

        protected override void Start()
        {
            base.Start();

            if (this.ActorType == ActorType.Normal)
            {
                foreach (var spawn in spawns)
                {
                    var go = Object.Instantiate(spawn);
                    go.transform.position = transform.position;
                    NetworkServer.Spawn(go);
                }
            }
          
            
        }
        
    }
}