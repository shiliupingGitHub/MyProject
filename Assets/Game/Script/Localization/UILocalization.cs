using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Extern
{
    public class UILocalization : MonoBehaviour
    {

        public string Key;
        private Text _text;
        private void Awake()
        {
            _text = GetComponent<Text>();
            Localize();
            LocalizationMgr.Instance.OnLanguageChanged += Localize;
        }

        void Localize()
        {
            if (null != _text)
            {
                _text.text = LocalizationMgr.Instance.Get(Key);
            }
        }

        private void OnDestroy()
        {
            LocalizationMgr.Instance.OnLanguageChanged += Localize;
        }
    }
}