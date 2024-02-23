using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Script.Character
{
    
    public class GamePlayerController : BaseController
    {
        public InputActionReference MoveUpAction;
        public InputActionReference MoveDownAction;
        public InputActionReference MoveLeftAction;
        public InputActionReference MoveRightAction;
        private void Update()
        {
            if (isOwned)
            {
                
            }
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            MoveUpAction.action.Enable();
            
            MoveUpAction.action.started += context =>
            {
                
            };
            MoveUpAction.action.canceled += context =>
            {
               
            };

        }
    }
}