using System.Collections.Generic;
using Game.Script.Attribute;
using Game.Script.Character;
using Game.Script.Common;
using Game.Script.Subsystem;
using Mirror;
using UnityEngine;

namespace Game.Script.Map.Actor
{
    public class MonsterSpawner : MapActor
    {
        [ActorDataDes("p1")]
        public int p1;
        [ActorDataDes("p2")]
        public float p2;
        [ActorDataDes("p3")]
        public string p3;
        public List<GameObject> spawns = new();

        void DoBorn()
        {
            foreach (var spawn in spawns)
            {
                var go = Object.Instantiate(spawn);
                var cellSize = Common.Game.Instance.MapBk.MyGrid.cellSize;
                go.transform.position = transform.position + new Vector3(cellSize.x, cellSize.y, 0);
                NetworkServer.Spawn(go);
            }
        }

        void TryDoBorn()
        {
            var fightSubsystem = Common.Game.Instance.GetSubsystem<FightSubsystem>();
            if (fightSubsystem.FightStart)
            {
                DoBorn();
            }
            else
            {
                fightSubsystem.fightStart += DoBorn;
            }
            
        }
        protected override void Start()
        {
            base.Start();

            if (this.ActorType == ActorType.Normal)
            {
                if (Common.Game.Instance.Mode == GameMode.Host)
                {
                    TryDoBorn();
                }
                
            }
          
            
        }
        
    }
}