using Game.Script.Common;
using Mirror;
using UnityEngine;

namespace Game.Script.Character
{
    [RequireComponent(typeof(NetworkTransformReliable))]
    public class Pawn : Actor
    {
        protected override void Awake()
        {
            base.Awake();
            Common.Game.Instance.RegisterPawn(this);
        }

        protected override void OnDestroy()  
        {
            base.OnDestroy();
            Common.Game.Instance.UnRegisterPawn(this);
        }
        
        public virtual void Tick(float deltaTime)
        {
        }
    }   
}