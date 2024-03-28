using System.Collections.Generic;
using Game.Script.Character;
using Mirror;
using UnityEngine;

namespace Game.Script.Map.Actor
{
    public class MonsterSpawner : MapActor
    {
        public List<GameObject> spawns = new();
        public override void OnStartServer()
        {
            base.OnStartServer();

            foreach (var spawn in spawns)
            {
                var go = Object.Instantiate(spawn);
                go.transform.position = transform.position;
                NetworkServer.Spawn(go);
            }
            
        }
    }
}