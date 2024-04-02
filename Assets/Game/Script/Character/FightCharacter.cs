using System;
using Cinemachine;
using Game.Script.Common;
using Game.Script.Subsystem;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Script.Character
{
    public class FightCharacter : Character
    {
        public InputActionReference MoveUpAction;
        public InputActionReference MoveDownAction;
        public InputActionReference MoveLeftAction;
        public InputActionReference MoveRightAction;
        public float MoveSpeed = 100;

        private Vector3 moveDir = Vector3.zero;
        private bool bInitCamera = false;
        private bool bCheckCamera = false;
        private Camera _camera;
        private Rigidbody2D _rigidbody;
        private CinemachineBrain _cinemachineBrain;

        [TargetRpc]
        void TargetRpc_SetStartFightLeftTime(float leftTime)
        {
            var fightSubsystem = Common.Game.Instance.GetSubsystem<FightSubsystem>();
            fightSubsystem.StartLeftTime = leftTime;
        }

        [Command]
        void Cmd_RequestEnterInfo()
        {
            var fightSubsystem = Common.Game.Instance.GetSubsystem<FightSubsystem>();
            TargetRpc_SetStartFightLeftTime(fightSubsystem.StartLeftTime);
        }

        private void OnUpdate(float deltaTime)
        {
            if (isLocalPlayer)
            {
                SetUpCamera();
                DoMove();
                
            }
        }
        
        protected override void Start()
        {
            base.Start();
            _rigidbody = GetComponent<Rigidbody2D>();

            GameLoop.Add(OnUpdate);

            if (isClientOnly)
            {
                Cmd_RequestEnterInfo();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            GameLoop.Remove(OnUpdate);
        }
        
        public override void OnStartAuthority()
        {
            base.OnStartAuthority();

            Common.Game.Instance.MyController = this;
        }

        void DoMove()
        {
            var dir = moveDir;
            dir.Normalize();

            _rigidbody.velocity = dir * MoveSpeed;
            if (null == _cinemachineBrain)
            {
                _cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
            }
            _cinemachineBrain.ManualUpdate();
        }

        void SetUpInput()
        {
            MoveUpAction.action.Enable();
            MoveDownAction.action.Enable();
            MoveLeftAction.action.Enable();
            MoveRightAction.action.Enable();

            MoveUpAction.action.started += context => { moveDir.y += 1; };
            MoveUpAction.action.canceled += context => { moveDir.y -= 1; };

            MoveDownAction.action.started += context => { moveDir.y -= 1; };
            MoveDownAction.action.canceled += context => { moveDir.y += 1; };

            MoveLeftAction.action.started += context => { moveDir.x -= 1; };
            MoveLeftAction.action.canceled += context => { moveDir.x += 1; };

            MoveRightAction.action.started += context => { moveDir.x += 1; };
            MoveRightAction.action.canceled += context => { moveDir.x -= 1; };
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