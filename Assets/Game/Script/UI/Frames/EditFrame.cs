using System;
using System.Collections.Generic;
using Game.Script.Common;
using Game.Script.Map;
using Game.Script.Misc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Script.UI.Frames
{
    public class EditFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/EditFrame.prefab";

        private Dropdown mapDropDown;

        private MapData _curMapData;
        private const float ZoomFactor = 400;
        private const float OrthographicSizeMin = 0.5f;
        private const float OrthographicSizeMax = 20;
        private const float MoveFactor = 0.01f;
        private bool bEnabledInput = false;
        private Dictionary<string, InputActionReference> _inputActionReferences = new();
        private bool bIsDraging = false;
        private bool bAddTick = false;
        private Vector3 lastDragPosition = Vector3.zero;
        void AddToTick()
        {
            if (!bAddTick)
            {
                GameTickManager.Instance.AddTick(Tick);
                bAddTick = true;
            }
        }

        void RemoveTick()
        {
            if (bAddTick)
            {
                GameTickManager.Instance.RemoveTick(Tick);
                bAddTick = false;
            }
        }

        void Tick()
        {
            if (bIsDraging)
            {
                TickDrag();
            }
            
        }

        void TickDrag()
        {
            if (_inputActionReferences.TryGetValue("EditDrag", out var inputDrag))
            {
                if (inputDrag.action.IsPressed())
                {
                    var newPosition = Input.mousePosition;
                    var delta = newPosition - lastDragPosition;

                    delta.z = 0;

                    var cameraPosition = Camera.main.transform.position;

                    cameraPosition -= delta * MoveFactor;
                    Camera.main.transform.position = cameraPosition;
                    lastDragPosition = newPosition;

                }
            }
        }
        void OnZoom(InputAction.CallbackContext callbackContext)
        {
            var delta = callbackContext.ReadValue<Vector2>();
            var orthographicSize = Camera.main.orthographicSize;
            
            orthographicSize -= delta.y / ZoomFactor;
            orthographicSize = Mathf.Clamp(orthographicSize, OrthographicSizeMin, OrthographicSizeMax);
            Camera.main.orthographicSize = orthographicSize;
        }

        void OnMove(InputAction.CallbackContext callbackContext)
        {
            var delta = callbackContext.ReadValue<Vector2>();
            var curPosition = Camera.main.transform.position;

            curPosition += new Vector3(delta.x * MoveFactor, delta.y * MoveFactor);
            Camera.main.transform.position = curPosition;
        }
        
        void InitInput()
        {
            // hookActions.Add("Zoom", OnZoom);
            // hookActions.Add("EditMove", OnMove);
            var hooks = FrameGo.GetComponents<InputActionHook>();
            foreach (var hook in hooks)
            {
                if (hook.inputAction != null)
                {
                    hook.inputAction.action.Enable();
                    _inputActionReferences.Add(hook.actionName, hook.inputAction);
                }
            }

            if (_inputActionReferences.TryGetValue("Zoom", out var inputZoom))
            {
                inputZoom.action.performed += OnZoom;
            }
            if (_inputActionReferences.TryGetValue("EditDrag", out var inputDrag))
            {
                inputDrag.action.started += delegate(InputAction.CallbackContext callbackContext)
                {
                    bIsDraging = true;
                    lastDragPosition = Input.mousePosition;
                };
                
                inputDrag.action.canceled += delegate(InputAction.CallbackContext callbackContext)
                {
                    bIsDraging = false;
                };
            }
        }

        void EnableInput()
        {
            if (!bEnabledInput)
            {

                foreach (var input in _inputActionReferences)
                {
                    input.Value.action.Enable();
                }
                bEnabledInput = true;
            }
           
        }

        void DisableInput()
        {
            if (bEnabledInput)
            {
                foreach (var input in _inputActionReferences)
                {
                    input.Value.action.Disable();
                }

                bEnabledInput = false;
            }
            
        }

        protected override void OnDestroy()
        {
            DisableInput();
            base.OnDestroy();
            RemoveTick();
            
        }

        void InitActors()
        {
            var actorTemplate = FrameGo.transform.Find("ActorTemplate").gameObject;
            var contentRoot = FrameGo.transform.Find("svActors/Viewport/Content");
            var actorConfigs = ActorConfig.dic;

            foreach (var actorConfig in actorConfigs)
            {
                var actorGo = Object.Instantiate(actorTemplate, contentRoot) as GameObject;
                actorGo.name = actorConfig.Value.id.ToString();
                var text = actorGo.transform.Find("Name").GetComponent<Text>();
                text.text = actorConfig.Value.name;
                actorGo.SetActive(true);
            }
        }

        void InitMaps()
        {
            mapDropDown = FrameGo.transform.Find("ddMap").GetComponent<Dropdown>();
            var mapConfigs = MapConfig.dic;

            List<string> mapDDContent = new();
            mapDropDown.ClearOptions();
            for (int i = 1; i <= mapConfigs.Count; i++)
            {
                var mapConfig = mapConfigs[i];
                
                mapDDContent.Add(mapConfig.name);
                
            }
            mapDropDown.AddOptions(mapDDContent);
        }
        public override void Init(Transform parent)
        {
            base.Init(parent);
            FrameGo.transform.Find("btnNew").GetComponent<Button>().onClick.AddListener(() =>
            {
                if (_curMapData != null)
                {
                    _curMapData.UnLoadSync();
                }
                _curMapData = MapMgr.Instance.New(mapDropDown.value + 1);
                _curMapData.LoadSync();
                EnableInput();
            });
            InitMaps();
            InitActors();
            InitInput();
            AddToTick();
        }
    }
}