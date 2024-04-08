using System.Reflection;
using Game.Script.Attribute;
using Game.Script.Res;
using Game.Script.Subsystem;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Ext
{
    public class FieldDrawer
    {
        private const string FloatTemplate = "Assets/Game/Res/UI/Extern/floatParam.prefab";
        private const string IntTemplate = "Assets/Game/Res/UI/Extern/intParam.prefab";
        private const string StringTemplate = "Assets/Game/Res/UI/Extern/stingParam.prefab";

        public static void BeginDraw(Transform tr)
        {
            for (int i = tr.childCount - 1; i >= 0; --i)
            {
                Object.Destroy(tr.GetChild(i).gameObject);
            }
        }
        public static void Draw(Transform tr, FieldInfo fieldInfo, object obj, System.Action<System.Object> onValueChanged)
        {
            if (fieldInfo.FieldType == typeof(float))
            {
                DrawFloat(tr, fieldInfo, obj, (float value) =>
                {
                    onValueChanged(value);
                });
            }
            else if (fieldInfo.FieldType == typeof(int))
            {
                DrawInt(tr, fieldInfo, obj, (int value) =>
                {
                    onValueChanged(value);
                });
            }
            else if (fieldInfo.FieldType == typeof(string))
            {
                DrawString(tr, fieldInfo, obj, (string value) =>
                {
                    onValueChanged(value);
                });
            }
        }
       static string GetHeaderName(FieldInfo fieldInfo)
        {
            string header = fieldInfo.Name;
            var attr = fieldInfo.GetCustomAttribute<LabelAttribute>();

            if (null != attr)
            {
                header = attr.Name;
            }
            
            var localizationSystem = Common.Game.Instance.GetSubsystem<LocalizationSubsystem>();

            return localizationSystem.Get(header);
        }
        public static void DrawFloat(Transform tr, FieldInfo fieldInfo, object obj, System.Action<float> onValueChanged)
        {
            var curValue = fieldInfo.GetValue(obj) is float ? (float)fieldInfo.GetValue(obj) : 0;
            string header = GetHeaderName(fieldInfo);
            var template = GameResMgr.Instance.LoadAssetSync<GameObject>(FloatTemplate);
            var go = Object.Instantiate(template, tr);
            var textName = go.transform.Find("tbName").GetComponent<Text>();
            textName.text = header;
            var input = go.transform.Find("inputValue").GetComponent<InputField>();
            input.text = curValue.ToString();

            input.onSubmit.AddListener(str =>
            {
                float value; 
                float.TryParse(str, result: out value);
                fieldInfo.SetValue(obj, value);
                onValueChanged.Invoke(value);
            });
        }
        public static void DrawString(Transform tr,FieldInfo fieldInfo, object obj, System.Action<string> onValueChanged)
        {
            var curValue = fieldInfo.GetValue(obj) is string ? (string)fieldInfo.GetValue(obj) : string.Empty;
            string header = GetHeaderName(fieldInfo);
            var template = GameResMgr.Instance.LoadAssetSync<GameObject>(StringTemplate);
            var go = Object.Instantiate(template, tr);
            go.SetActive(true);
            var textName = go.transform.Find("tbName").GetComponent<Text>();
            textName.text = header;
            var input = go.transform.Find("inputValue").GetComponent<InputField>();
            input.text = curValue;

            input.onSubmit.AddListener(str =>
            {
                fieldInfo.SetValue(obj, str);
                onValueChanged.Invoke(str);
            });
        }
        public static void DrawInt(Transform tr,FieldInfo fieldInfo, object obj, System.Action<int> onValueChanged)
        {
            var curValue = fieldInfo.GetValue(obj) is int ? (int)fieldInfo.GetValue(obj) : 0;
            var template = GameResMgr.Instance.LoadAssetSync<GameObject>(IntTemplate);
            var go = Object.Instantiate(template, tr);
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
                fieldInfo.SetValue(obj, value);
                onValueChanged.Invoke(value);
            });
        }
    }
}