using System;
using System.Collections.Generic;
using System.IO;
using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Map;
using Game.Script.Misc;
using Game.Script.Res;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Script.UI.Frames
{
    public class EditFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/EditFrame.prefab";
        [UIPath("ddBk")]
        private Dropdown ddBk;

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
        private ActorConfig curSelectActorConfig;
        private GameObject curSelectShadow;
        [UIPath("InputSaveName")]
        private InputField inputSaveName;
        [UIPath("Load/ddSaveMaps")] private Dropdown ddSaveMaps;
        [UIPath("btnNew")] private Button btnNew;
        [UIPath("Load/btnLoad")] private Button btnLoad;
        [UIPath("btnSave")] private Button btnSave;
        [UIPath("ActorTemplate")] private GameObject actorTemplate;
        [UIPath("svActors/Viewport/Content")] private Transform contentRoot;
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
            TickShadow();
            
        }

        void TickShadow()
        {
            if (curSelectShadow != null)
            {
                var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                MapScript mapScript = GameObject.FindObjectOfType<MapScript>();

                if (null != mapScript)
                {
                    curSelectShadow.transform.position =  mapScript.ConvertToGridPosition(worldPosition);
                }
                
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
                    _inputActionReferences.Add(hook.inputAction.action.name, hook.inputAction);
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

            if (_inputActionReferences.TryGetValue("EditAddActor", out var inputAddActor))
            {
                inputAddActor.action.started += delegate(InputAction.CallbackContext callbackContext)
                {
                    if (null != curSelectShadow && null != curSelectActorConfig && null != _curMapData)
                    {
                        _curMapData.AddActor(curSelectShadow.transform.position,curSelectActorConfig);
                    }
                };
            }

            if (_inputActionReferences.TryGetValue("CancelEditActor", out var inputCancelEditActor))
            {
                inputCancelEditActor.action.started += delegate(InputAction.CallbackContext callbackContext)
                {
                   SetSelectActor(null);
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
            var actorConfigs = ActorConfig.dic;
            foreach (var actorConfig in actorConfigs)
            {
                var actorGo = Object.Instantiate(actorTemplate, contentRoot) as GameObject;
                actorGo.transform.localScale = Vector3.one;
                actorGo.name = actorConfig.Value.id.ToString();
                var text = actorGo.transform.Find("Name").GetComponent<Text>();
                text.text = actorConfig.Value.name;
                actorGo.SetActive(true);
                var btn = actorGo.GetComponent<Button>();

                if (btn != null)
                {
                    btn.onClick.AddListener(() =>
                    {
                       
                        SetSelectActor(actorConfig.Value);
                    });
                }
            }
        }

        void SetSelectActor(ActorConfig actorConfig)
        {
            if (null != curSelectShadow)
            {
                GameObject.Destroy(curSelectShadow);
                curSelectShadow = null;
                curSelectActorConfig = null;
            }

            if (actorConfig != null)
            {
                var template = GameResMgr.Instance.LoadAssetSync<GameObject>(actorConfig.path);

                if (template)
                {
                    curSelectShadow = GameObject.Instantiate(template) as GameObject;
                    curSelectActorConfig = actorConfig;
                    curSelectShadow.tag = "Shadow";

                    var actor = curSelectShadow.GetComponent<Actor>();

                    if (null != actor)
                    {
                        actor.ActorType = ActorType.Shadow;
                    }
                }
            }
            
        }

        void InitBks()
        {
            var mapConfigs = MapBKConfig.dic;

            List<string> mapDDContent = new();
            ddBk.ClearOptions();
            for (int i = 1; i <= mapConfigs.Count; i++)
            {
                var mapConfig = mapConfigs[i];
                
                mapDDContent.Add(mapConfig.name);
                
            }
            ddBk.AddOptions(mapDDContent);
        }

        void InitSavedMaps()
        {
            RefreshSaveMaps();
        }

        void RefreshSaveMaps()
        {
            ddSaveMaps.ClearOptions();
            List<string> allMaps = new();
            var path = SavePath;
            var files = Directory.GetFiles(path);

            foreach (var file in files)
            {
                if (file.EndsWith(MapExtension))
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    allMaps.Add(fileName);
                }
            }
            ddSaveMaps.AddOptions(allMaps);
        }

        public string SavePath
        {
            get
            {
                string path = System.IO.Path.Combine(Application.persistentDataPath, "Map");
                if (Application.isEditor)
                {
                    path = System.IO.Path.Combine(Application.dataPath, "Game/Res/Map/Data");
                }

                return path;
            }
        }

        public string MapExtension
        {
            get
            {
                return ".txt";
            }
        }

        void SetCameraCenter(MapScript mapScript)
        {
            Grid grid = mapScript.MyGrid;

            if (grid != null)
            {
                Vector3 center = mapScript.transform.position;
                center += new Vector3(grid.cellSize.x * mapScript.xGridNum * 0.5f, grid.cellSize.y * mapScript.yGridNum * 0.5f, 0);

                Vector3 cameraPosition = Camera.main.transform.position;
                cameraPosition.x = center.x;
                cameraPosition.y = center.y;
                Camera.main.transform.position = cameraPosition;

            }
        }
        public override void Init(Transform parent)
        {
            base.Init(parent);
            btnNew.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (_curMapData != null)
                {
                    _curMapData.UnLoadSync();
                }
                _curMapData = MapMgr.Instance.New(ddBk.value + 1);
                _curMapData.LoadSync();
                EnableInput();
                MapScript mapScript = GameObject.FindObjectOfType<MapScript>();
                if (mapScript != null)
                {
                    SetCameraCenter(mapScript);
                    mapScript.showGrid = true;
                }
                
            });
            
            btnLoad.onClick.AddListener(() =>
            {
                if (_curMapData != null)
                {
                    _curMapData.UnLoadSync();
                    _curMapData = null;
                }

                var fileName = ddSaveMaps.captionText.text;

                var path = Path.Combine(SavePath, fileName + MapExtension);

                if (File.Exists(path))
                {
                    var data = File.ReadAllText(path);
                    var mapData = MapData.DeSerialize(data);

                    if (MapBKConfig.dic.ContainsKey(mapData.bkId))
                    {
                        _curMapData = mapData;
                        _curMapData.LoadSync();
                        MapScript mapScript = GameObject.FindObjectOfType<MapScript>();

                        if (mapScript != null)
                        {
                            EnableInput();
                            SetCameraCenter(mapScript);
                            mapScript.showGrid = true;
                            inputSaveName.text = fileName;
                        }
                    }
                }

            });
            
            btnSave.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (_curMapData != null && null != inputSaveName)
                {
                    if (!string.IsNullOrEmpty(inputSaveName.text))
                    {
                        var data = _curMapData.Serialize();
                        string path = SavePath;
                        
                        path = System.IO.Path.Combine(path, inputSaveName.text + MapExtension);
         
                        File.WriteAllText(path, data);
#if UNITY_EDITOR
                        UnityEditor.AssetDatabase.Refresh();
#endif
                        RefreshSaveMaps();
                    }
                }
               
            });
            
            InitBks();
            InitSavedMaps();
            InitActors();
            InitInput();
            AddToTick();
        }
    }
}