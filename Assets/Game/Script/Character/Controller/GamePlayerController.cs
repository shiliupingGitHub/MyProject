using System;
using Game.Script.Game;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Script.Character
{
    public class GamePlayerController : BaseSkillController
    {
        public InputActionReference MoveUpAction;
        public InputActionReference MoveDownAction;
        public InputActionReference MoveLeftAction;
        public InputActionReference MoveRightAction;
        public float MoveSpeed = 1;
        
        private Vector3 moveDir = Vector3.zero;
        private bool bInitCamera = false;
        private bool bCheckCamera = false;
        private Camera _camera;
        private Rigidbody2D _rigidbody;
        private void Update()
        {
            if (isLocalPlayer)
            {
               SetUpCamera();
            }
        }

        private void FixedUpdate()
        {
            if (isLocalPlayer)
            {
                DoMove();
            }
        }

        protected override void Start()
        {
            base.Start();
            _rigidbody = GetComponent<Rigidbody2D>();
        }


        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            Game.Game.Instance.MyController = this;
        }

        void DoMove()
        {
            var dir = moveDir;
            dir.Normalize();
            
            _rigidbody.velocity = dir * MoveSpeed * Time.deltaTime;

        }

        void SetUpInput()
        {
            MoveUpAction.action.Enable();
            MoveDownAction.action.Enable();
            MoveLeftAction.action.Enable();
            MoveRightAction.action.Enable();
            
            MoveUpAction.action.started += context =>
            {
                moveDir.y += 1;
            };
            MoveUpAction.action.canceled += context =>
            {
                moveDir.y -= 1;
            };
            
            MoveDownAction.action.started += context =>
            {
                moveDir.y -= 1;
            };
            MoveDownAction.action.canceled += context =>
            {
                moveDir.y += 1;
            };
            
            MoveLeftAction.action.started += context =>
            {
                moveDir.x -= 1;
            };
            MoveLeftAction.action.canceled += context =>
            {
                moveDir.x += 1;
            };
            
            MoveRightAction.action.started += context =>
            {
                moveDir.x += 1;
            };
            MoveRightAction.action.canceled += context =>
            {
                moveDir.x -= 1;
            };
        }

        void SetUpCamera()
        {
            if (!bInitCamera)
            {
                if (bCheckCamera)
                {
                    var mainCamera = Camera.main;

                    if (mainCamera)
                    {
                        mainCamera.transform.SetParent(transform);

                        mainCamera.transform.localPosition = Vector3.zero;
                        mainCamera.transform.localEulerAngles = Vector3.zero;
                        bInitCamera = true;
                    }
                }
            }
            
        }

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            SetUpInput();
            bCheckCamera = true;
        }
        
    }
}
