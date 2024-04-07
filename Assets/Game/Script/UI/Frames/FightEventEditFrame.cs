
using Game.Script.Attribute;
using Game.Script.Common;
using Game.Script.Map;
using OneP.InfinityScrollView;
using UnityEngine;
using UnityEngine.UI;

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

        [UIPath("offset/btnTimeEvent")] private Button _btnTimeEvent;
        [UIPath("offset/btnSystemEvent")] private Button _btnSystemEvent;
        [UIPath("offset/btnCustomEvent")] private Button _btnCustomEvent;
        [UIPath("offset/eventList")] private InfinityScrollView _eventList;

        [UIPath("offset/Content/inputEventName")]
        private InputField _inputEventName;

        [UIPath("offset/Content/inputTime")] private InputField _inputTime;
        [UIPath("offset/Content/actionList")] private InfinityScrollView _actionList;

        [UIPath("offset/Content/inputActionName")]
        private InputField _inputActionName;

        [UIPath("offset/Content/params")] private Transform paramRoot;
        [UIPath("offset/Content/floatParam")] private GameObject _floatPram;
        [UIPath("offset/Content/stingParam")] private GameObject _stringPram;
        [UIPath("offset/Content/intParam")] private GameObject _intPram;

        private int _curEventPage = 0;
        private int _curSelectEvent = -1;
        private MapData _curMapData;
        private bool bRefreshEventList = false;

        public override void Init(Transform parent)
        {
            base.Init(parent);
            _btnClose.onClick.AddListener(Hide);
            InitEventList();
            InitBtns();
            GameLoop.Add(OnUpdate);
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
        }

        public void SetData(MapData mapData)
        {
            _curMapData = mapData;

            RefreshEventList();
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
                        var te = new MapTimeEvent();
                        te.name = "new time event";
                        _curMapData.timeEvents.Add(te);
                    }
                        break;
                    case 1:
                    {
                        var e = new MapSystemEvent();
                        e.name = "new sytem event";
                        _curMapData.systemEvents.Add(e);
                    }
                        break;
                    case 2:
                    {
                        var e = new MapCustomEvent();
                        e.name = "new custom event";
                        _curMapData.customEvents.Add(e);
                    }
                        break;
                }
                RefreshEventList();
            });
            
            
        }

        void InitEventList()
        {
            _eventList.onItemReload += (go, index) =>
            {
                MapEvent ed = null;
                Text text = go.transform.Find("Text").GetComponent<Text>();
                switch (_curEventPage)
                {
                    case 0:
                        ed = _curMapData.timeEvents[index];
                        break;
                    case 1:
                        ed = _curMapData.systemEvents[index];
                        break;
                    case 2:
                        ed = _curMapData.customEvents[index];
                        break;
                }
                var btn = go.GetComponent<Button>();
                var image = go.GetComponent<Image>();
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    _curSelectEvent = index;
                    bRefreshEventList = true;
                });

                text.text = ed.name;
                image.color = index == _curSelectEvent?Color.green:Color.white;
            };
        }
    }
}