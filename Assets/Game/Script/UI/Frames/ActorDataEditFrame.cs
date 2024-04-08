using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Script.Attribute;
using Game.Script.Map;
using Game.Script.Map.Actor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Script.UI.Frames
{
    public class ActorDataEditFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/ActorDataEditFrame.prefab";
        [UIPath("offset/btnClose")] private Button _btnClose;
        [UIPath("offset/btnOK")] private Button _btnOK;
        [UIPath("offset/params")] private Transform _paramsRoot;
        [UIPath("offset/floatParam")] private GameObject _floatParam;
        [UIPath("offset/intParam")] private GameObject _intParam;
        [UIPath("offset/stringParam")] private GameObject _stringParam;
        private ActorData _curActorData;
        private Dictionary<System.Type, Action<System.Object, FieldInfo>> _typeDraw = new();

        public override void Init(Transform parent)
        {
            base.Init(parent);
            _btnClose.onClick.AddListener(() => { Hide(); });
            _btnOK.onClick.AddListener(() => { Hide(); });
            _typeDraw.Add(typeof(string), OnDrawStringField);
            _typeDraw.Add(typeof(float), OnDrawFloatField);
            _typeDraw.Add(typeof(int), OnDrawIntField);
        }

        string GetHeaderName(FieldInfo fieldInfo)
        {
            string header = fieldInfo.Name;
            var attr = fieldInfo.GetCustomAttribute<ActorDataDesAttribute>();

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
            var go = Object.Instantiate(_stringParam, _paramsRoot);
            go.SetActive(true);
            var textName = go.transform.Find("tbName").GetComponent<Text>();
            textName.text = header;
            var input = go.transform.Find("inputValue").GetComponent<InputField>();
            input.text = curValue;

            input.onSubmit.AddListener(str =>
            {
                fieldInfo.SetValue(action, str);
                _curActorData.Set(fieldInfo.Name, str);
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
            var go = Object.Instantiate(_floatParam, _paramsRoot);
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
                _curActorData.Set(fieldInfo.Name, value);
            });
        }

        void OnDrawIntField(System.Object action, FieldInfo fieldInfo)
        {
            if (null == action)
            {
                return;
            }

            var curValue = fieldInfo.GetValue(action) is int ? (int)fieldInfo.GetValue(action) : 0;
            var go = Object.Instantiate(_intParam, _paramsRoot);
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
                _curActorData.Set(fieldInfo.Name, value);
            });
        }

        public void SetActorData(ActorData actorData)
        {
            _curActorData = actorData;
            RefreshActorUI();
        }

        void RefreshActorUI()
        {
            for (int i = _paramsRoot.childCount - 1; i >= 0; --i)
            {
                Object.Destroy(_paramsRoot.GetChild(i).gameObject);
            }

            if (_curActorData.go == null)
            {
                return;
            }

            var mapActor = _curActorData.go.GetComponent<MapActor>();

            if (null == mapActor)
            {
                return;
            }
            
           
            var typeInfo = (System.Reflection.TypeInfo)mapActor.GetType();
            
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
                
                var attr = field.GetCustomAttribute<ActorDataDesAttribute>();

                if (null != attr)
                {
                    if (_typeDraw.TryGetValue(fieldType, out var drawAction))
                    {
                        drawAction.Invoke(mapActor, field);
                    }
                }
              
                
            }
        }
    }
}