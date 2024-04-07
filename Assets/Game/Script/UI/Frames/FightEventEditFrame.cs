
using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Map;
using Game.Script.Subsystem;
using OneP.InfinityScrollView;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Script.UI.Frames
{
    public class FightEventEditFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/FightEventEditFrame.prefab";
        [UIPath("offset/btnClose")] private Button _btnClose;
        [UIPath("offset/btnAddEvent")] private Button _btnAddEvent;
        [UIPath("offset/btnRemoveEvent")] private Button _btnRemoveEvent;
        [UIPath("offset/Content/btnAddAction")]
        private Button _btnAddAction;
        [UIPath("offset/Content/btnRemoveAction")]
        private Button _btnRemoveAction;

        [UIPath("offset/btnTimeEvent")] private Button _btnTimeEvent;
        [UIPath("offset/btnSystemEvent")] private Button _btnSystemEvent;
        [UIPath("offset/btnCustomEvent")] private Button _btnCustomEvent;
        [UIPath("offset/eventList")] private InfinityScrollView _eventList;

        [UIPath("offset/Content/inputEventName")]
        private InputField _inputEventName;

        [UIPath("offset/Content/inputTime")] private InputField _inputTime;
        [UIPath("offset/Content/actionList")] private InfinityScrollView _actionList;

        [UIPath("offset/Content/ActionDetail/inputActionName")]
        private InputField _inputActionName;

        [UIPath("offset/Content/ActionDetail/params")] private Transform paramRoot;
        [UIPath("offset/Content/floatParam")] private GameObject _floatPram;
        [UIPath("offset/Content/stingParam")] private GameObject _stringPram;
        [UIPath("offset/Content/intParam")] private GameObject _intPram;
        [UIPath("offset/Content/ActionDetail/ddActionType")] private Dropdown _ddActionType;

        [UIPath("offset/Content/ActionDetail")]
        private GameObject _actionDetail;

        private int _curEventPage = 0;
        private int _curSelectEvent = -1;
        private MapData _curMapData;
        private bool bRefreshEventList = false;
        private bool bRefreshActionList = false;
        private bool bRefreshActionDetail = true;
        private int _curSelectAction = -1;
        private bool bSerilizeAction = false;
        private MapActionData _curActionData;
        private MapAction _drawAction = null;
        private Dictionary<System.Type, Action<System.Object, FieldInfo>> _typeDraw = new();
        

        public override void Init(Transform parent)
        {
            base.Init(parent);
            _btnClose.onClick.AddListener(Hide);
            InitEventList();
            InitActionList();
            InitBtns();
            GameLoop.Add(OnUpdate);
            _typeDraw.Add(typeof(string), OnDrawStringField);
            _typeDraw.Add(typeof(float), OnDrawFloatField);
            _typeDraw.Add(typeof(int), OnDrawIntField);
        }
        
        string GetHeaderName(FieldInfo fieldInfo)
        {
            string header = fieldInfo.Name;
            var attr = fieldInfo.GetCustomAttribute<LabelAttribute>();

            if (null != attr)
            {
                header = attr.Name;
            }

            return header;
        }
        void OnDrawStringField(System.Object action, FieldInfo fieldInfo)
        {
            if (null == action)
            {
                return;
            }
            var curValue = fieldInfo.GetValue(action) is string ? (string)fieldInfo.GetValue(action) : string.Empty;
            string header = GetHeaderName(fieldInfo);
            var go = Object.Instantiate(_stringPram, paramRoot);
            go.SetActive(true);
            var textName = go.transform.Find("tbName").GetComponent<Text>();
            textName.text = header;
            var input = go.transform.Find("inputValue").GetComponent<InputField>();
            input.text = curValue;

            input.onSubmit.AddListener(str =>
            {
                fieldInfo.SetValue(action, str);
                bSerilizeAction = true;
            });
       

        }
        
        void OnDrawFloatField(System.Object action, FieldInfo fieldInfo)
        {
            if (null == action)
            {
                return;
            }
            var curValue = fieldInfo.GetValue(action) is float ? (float)fieldInfo.GetValue(action) : 0;
            string header = GetHeaderName(fieldInfo);
            var go = Object.Instantiate(_floatPram, paramRoot);
            go.SetActive(true);
            var textName = go.transform.Find("tbName").GetComponent<Text>();
            textName.text = header;
            var input = go.transform.Find("inputValue").GetComponent<InputField>();
            input.text = curValue.ToString();

            input.onSubmit.AddListener(str =>
            {
                float value; 
                float.TryParse(str, result: out value);
                fieldInfo.SetValue(action, value);
                bSerilizeAction = true;
            });
            
        }
        
        void OnDrawIntField(System.Object action, FieldInfo fieldInfo)
        {
            if (null == action)
            {
                return;
            }
            var curValue = fieldInfo.GetValue(action) is int ? (int)fieldInfo.GetValue(action) : 0;
            var go = Object.Instantiate(_intPram, paramRoot);
            go.SetActive(true);
            string header = GetHeaderName(fieldInfo);
            var textName = go.transform.Find("tbName").GetComponent<Text>();
            textName.text = header;
            var input = go.transform.Find("inputValue").GetComponent<InputField>();
            input.text = curValue.ToString();

            input.onSubmit.AddListener(str =>
            {
                int value; 
                int.TryParse(str, result: out value);
                fieldInfo.SetValue(action, value);
                bSerilizeAction = true;
            });
            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameLoop.Remove(OnUpdate);
        }

        void OnUpdate(float delta)
        {
            if (bRefreshEventList)
            {
                bRefreshEventList = false;
                RefreshEventList();
            }
            
            if(bRefreshActionList)
            {
                bRefreshActionList = false;
                RefreshActionList();
                RefreshCurSelectEvent();
            }
            if(bRefreshActionDetail)
            {
                bRefreshActionDetail = false;
                RefreshActionDetail();
            }

            if (bSerilizeAction)
            {
                bSerilizeAction = false;
                SerializeAction();
            }
        }

        void SerializeAction()
        {
            if(_curActionData == null)
            {
                return;
            }
            
            if(null == _drawAction)
            {
                return;
            }

            _curActionData.data = JsonUtility.ToJson(_drawAction);

        }

        public void SetData(MapData mapData)
        {
            _curMapData = mapData;

            RefreshEventList();
            RefreshActionList();
        }

        void RefreshActionDetail()
        {
            if (_curActionData == null)
            {
                _actionDetail.SetActive(false);
                return;
            }
            _actionDetail.SetActive(true);
            _inputActionName.text = _curActionData.name;
            _inputActionName.onSubmit.RemoveAllListeners();
            _inputActionName.onSubmit.AddListener(str =>
            {
                _curActionData.name = str;
                bRefreshActionList = true;
            });
            _ddActionType.ClearOptions();
            List<string> options = new List<string>();
            foreach (var actionType in Enum.GetValues(typeof(MapActionType)))
            {
                options.Add(actionType.ToString());
            }
            _ddActionType.AddOptions(options);
            _ddActionType.onValueChanged.RemoveAllListeners();
            _ddActionType.onValueChanged.AddListener(index =>
            {
                var str = options[index];
                _curActionData.type =  Enum.Parse<MapActionType>(str);
                bRefreshActionDetail = true;
            });
            
            for (int i = paramRoot.childCount - 1; i >= 0; --i)
            {
                Object.Destroy(paramRoot.GetChild(i).gameObject);
            }
            _ddActionType.value = (int)_curActionData.type;
            DrawAction(_curActionData.type, _curActionData.data);

        }
        
        void DrawAction(MapActionType actionType, string param)
        {
            var mapActionSubsystem = Common.Game.Instance.GetSubsystem<MapEventSubsystem>();
            var type = mapActionSubsystem.ActionTypes[actionType];
            _drawAction = JsonUtility.FromJson(param, type) as MapAction;

            if (null == _drawAction)
            {
                _drawAction = System.Activator.CreateInstance(type) as MapAction;
            }
            var typeInfo = (System.Reflection.TypeInfo)type;
            
            foreach (var field in typeInfo.DeclaredFields)
            {
                if (field.IsStatic)
                {
                    continue;
                }

                if (!field.IsPublic)
                {
                    continue;
                }
                var fieldType = field.FieldType;

                if (_typeDraw.TryGetValue(fieldType, out var drawAction))
                {
                    drawAction.Invoke(_drawAction, field);
                }
                
            }
        }

        void RefreshActionList()
        {
            if (_curSelectEvent < 0)
            {
                _actionList.Setup(0);
                return;
            }

            switch (_curEventPage)
            {
                case 0:
                    RefreshTimeActionList();
                    break;
                case 1:
                    RefreshSystemActionList();
                    break;
                case 2:
                    RefreshCustomActionList();
                    break;
            }
            
        }

        void RefreshTimeActionList()
        {
            _actionList.Setup(_curMapData.timeEvents[_curSelectEvent].actions.Count);
        }
        
        void RefreshSystemActionList()
        {
            _actionList.Setup(_curMapData.systemEvents[_curSelectEvent].actions.Count);
        }
        
        void RefreshCustomActionList()
        {
            _actionList.Setup(_curMapData.customEvents[_curSelectEvent].actions.Count);
        }

        void RefreshEventList()
        {
            if (null == _curMapData)
            {
                return;
            }

            switch (_curEventPage)
            {
                case 0:
                {
                    _eventList.Setup(_curMapData.timeEvents.Count);
                    _inputTime.gameObject.SetActive(true);
                }
                    break;
                case 1:
                {
                    _eventList.Setup(_curMapData.systemEvents.Count);
                    _inputTime.gameObject.SetActive(false);
                }
                    break;
                case 2:
                {
                    _eventList.Setup(_curMapData.customEvents.Count);
                    _inputTime.gameObject.SetActive(false);
                }
                    break;
            }
        }
        

        void InitBtns()
        {
            _btnTimeEvent.onClick.AddListener(() =>
            {
                _curEventPage = 0;
                RefreshEventList();
            });
            _btnSystemEvent.onClick.AddListener(() =>
            {
                _curEventPage = 1;
                RefreshEventList();
            });
            _btnCustomEvent.onClick.AddListener(() =>
            {
                _curEventPage = 2;
                RefreshEventList();
            });
            _btnRemoveEvent.onClick.AddListener(() =>
            {
                if (_curSelectEvent > 0)
                {
                    switch (_curEventPage)
                    {
                        case 0:
                            _curMapData.timeEvents.RemoveAt(_curSelectEvent);
                            break;
                        case 1:
                            _curMapData.systemEvents.RemoveAt(_curSelectEvent);
                            break;
                        case 2:
                            _curMapData.customEvents.RemoveAt(_curSelectEvent);
                            break;
                    }
                    RefreshEventList();
                }
            });
            _btnAddEvent.onClick.AddListener(() =>
            {
                switch (_curEventPage)
                {
                    case 0:
                    {
                        var te = new MapTimeEventData();
                        te.name = "new time event";
                        _curMapData.timeEvents.Add(te);
                    }
                        break;
                    case 1:
                    {
                        var e = new MapSystemEventData();
                        e.name = "new sytem event";
                        _curMapData.systemEvents.Add(e);
                    }
                        break;
                    case 2:
                    {
                        var e = new MapCustomEventData();
                        e.name = "new custom event";
                        _curMapData.customEvents.Add(e);
                    }
                        break;
                }
                RefreshEventList();
            });
            
            _btnRemoveAction.onClick.AddListener(() =>
            {
                if (_curSelectEvent > 0)
                {
                    switch (_curEventPage)
                    {
                        case 0:
                            _curMapData.timeEvents[_curSelectEvent].actions.RemoveAt(_curSelectAction);
                            break;
                        case 1:
                            _curMapData.systemEvents[_curSelectEvent].actions.RemoveAt(_curSelectAction);
                            break;
                        case 2:
                            _curMapData.customEvents[_curSelectEvent].actions.RemoveAt(_curSelectAction);
                            break;
                    }

                    bRefreshActionList = true;
                }
            });
            _btnAddAction.onClick.AddListener(() =>
            {
                bRefreshActionList = true;
                var action = new MapActionData();
                action.name = "new action";
                action.type = MapActionType.BornMonster;
                switch (_curEventPage)
                {
                    case 0:
                    {
                     
                        _curMapData.timeEvents[_curSelectEvent].actions.Add(action);
                    }
                        break;
                    case 1:
                    {
                      _curMapData.systemEvents[_curSelectEvent].actions.Add(action);
                    }
                        break;
                    case 2:
                    {
                        _curMapData.customEvents[_curSelectEvent].actions.Add(action);
                    }
                        break;
                }
                
            });
            
            
        }

        void InitActionList()
        {
            _actionList.onItemReload += (go, index) =>
            {
                MapActionData data = null;
                Text text = go.transform.Find("Text").GetComponent<Text>();
                switch (_curEventPage)
                {
                    case 0:
                        data = _curMapData.timeEvents[_curSelectEvent].actions[index];
                        break;
                    case 1:
                        data  = _curMapData.systemEvents[_curSelectEvent].actions[index];
                        break;
                    case 2:
                        data  = _curMapData.customEvents[_curSelectEvent].actions[index];
                        break;
                }
                var btn = go.GetComponent<Button>();
                var image = go.GetComponent<Image>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    _curSelectAction = index;
                    bRefreshActionList = true;
                    _curActionData = data;
                    bRefreshActionDetail = true;
                });

                text.text = data.name;
                image.color = index == _curSelectAction?Color.green:Color.white;
            };
        }

        void ChangeSelectEventName(string name)
        {
            MapEventData ed = null;
            switch (_curEventPage)
            {
                case 0:
                {
                    ed = _curMapData.timeEvents[_curSelectEvent];
                }
                    break;
                case 1:
                {
                    ed = _curMapData.systemEvents[_curSelectEvent];
                }
                    break;
                case 2:
                {
                    ed = _curMapData.customEvents[_curSelectEvent];
                }
                    break;
            }
            ed.name = name;
        }

        void ChangeEventTime(float time)
        {
            if (_curEventPage != 0)
            {
                return;
            }
            MapTimeEventData ed  = _curMapData.timeEvents[_curSelectEvent];
            ed.time = time;

        }

        void RefreshCurSelectEvent()
        {
            MapEventData ed = null;
            switch (_curEventPage)
            {
                case 0:
                {
                    ed = _curMapData.timeEvents[_curSelectEvent];
                }
                    break;
                case 1:
                {
                    ed = _curMapData.systemEvents[_curSelectEvent];
                }
                    break;
                case 2:
                {
                    ed = _curMapData.customEvents[_curSelectEvent];
                }
                    break;
            }
            if (_curEventPage == 0)
            {
                _inputTime.text = ((MapTimeEventData) (ed)).time.ToString();
                _inputTime.onSubmit.RemoveAllListeners();
                _inputTime.onSubmit.AddListener(str =>
                {
                    if (float.TryParse(str, out var time))
                    {
                        ChangeEventTime(time);
                    }
                           
                });
            }
                    
            _inputEventName.text = ed.name;
            _inputEventName.onSubmit.RemoveAllListeners();
            _inputEventName.onSubmit.AddListener(str =>
            {
                ChangeSelectEventName(str);
                bRefreshEventList = true;
            });
        }
        void InitEventList()
        {
           
            _eventList.onItemReload += (go, index) =>
            {
                MapEventData ed = null;
                Text text = go.transform.Find("Text").GetComponent<Text>();
                switch (_curEventPage)
                {
                    case 0:
                    {
                        ed = _curMapData.timeEvents[index];
                    }
                        break;
                    case 1:
                    {
                        ed = _curMapData.systemEvents[index];
                    }
                        break;
                    case 2:
                    {
                        ed = _curMapData.customEvents[index];
                    }
                        break;
                }
                var btn = go.GetComponent<Button>();
                var image = go.GetComponent<Image>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    _curSelectEvent = index;
                    bRefreshEventList = true;
                    bRefreshActionList = true;
                    

                });
                
                text.text = ed.name;
                image.color = index == _curSelectEvent?Color.green:Color.white;
            };
        }
    }
}