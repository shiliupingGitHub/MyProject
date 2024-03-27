using Game.Script.Common;
using Game.Script.Game;
using Mirror;
using UnityEngine;

namespace Game.Script.Character
{
    [RequireComponent(typeof(NetworkTransformReliable))]
    public class BaseController : Actor
    {
        public virtual void Awake()
        {
            Game.Game.Instance.RegisterController(this);
        }

        protected override void OnDestroy()  
        {
            base.OnDestroy();
            Game.Game.Instance.UnRegisterController(this);
        }
        
        public virtual void Tick(float deltaTime)
        {
        }
    }   
}