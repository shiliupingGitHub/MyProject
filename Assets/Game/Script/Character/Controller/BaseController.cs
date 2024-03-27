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
            GameInstance.Instance.RegisterController(this);
        }

        protected override void OnDestroy()  
        {
            base.OnDestroy();
            GameInstance.Instance.UnRegisterController(this);
        }
        
        public virtual void Tick(float deltaTime)
        {
        }
    }   
}