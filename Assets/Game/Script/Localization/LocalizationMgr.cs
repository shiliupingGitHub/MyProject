using System.Collections.Generic;
using Game.Script.Common;
using Game.Script.Res;
using UnityEngine;

namespace Game.Script.UI.Extern
{
    
    public class LocalizationMgr : Singleton<LocalizationMgr>
    {
        private Dictionary<SystemLanguage, LanguageData> _languageDatas = new();

        public System.Action OnLanguageChanged;
        private SystemLanguage curLanguage = SystemLanguage.English;
        public void Init()
        {
            var languageGo =GameResMgr.Instance.LoadAssetSync<GameObject>("Assets/Game/Res/Localization/LocalizationConfig.prefab");
            var localizationConfig = languageGo.GetComponent<LocalizationConfig>();

            for (int i = 0; i < localizationConfig._keys.Count; i++)
            {
                var key = localizationConfig._keys[i];
                var value = localizationConfig._values[i];
                var content = System.Text.Encoding.GetEncoding("GBK").GetString(value.bytes);
                LanguageData data = new LanguageData();
                data.Load(content);
                _languageDatas.Add(key, data);
            }
        }

        public void SetLanguage(SystemLanguage language)
        {
            if (curLanguage != language)
            {
                curLanguage = language;
                if (null != OnLanguageChanged)
                {
                    OnLanguageChanged.Invoke();
                }
            }
        }
        

        public string Get(string key)
        {
            if (_languageDatas.TryGetValue(curLanguage, out var data))
            {
                if (data.dic.TryGetValue(key, out var ret))
                {
                    return ret.data;
                }
            }
            return key;
        }
    }
}