using Game.Script.Common;
using Game.Script.Game;
using Mirror;
using UnityEngine;

namespace Game.Script.Character
{
    [RequireComponent(typeof(NetworkTransformReliable))]
    public class Pawn : Actor
    {
        public virtual void Awake()
        {
            Game.Game.Instance.RegisterPawn(this);
        }

        protected override void OnDestroy()  
        {
            base.OnDestroy();
            Game.Game.Instance.UnRegisterPawn(this);
        }
        
        public virtual void Tick(float deltaTime)
        {
        }
    }   
}