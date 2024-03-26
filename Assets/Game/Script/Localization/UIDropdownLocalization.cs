﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Extern
{
    public class UIDropdownLocalization : MonoBehaviour
    {
        private Dropdown _dropdown;
        public List<string> Keys;
        private void Awake()
        {
            _dropdown = GetComponent<Dropdown>();
            Localize();
            LocalizationMgr.Instance.OnLanguageChanged += Localize;
        }

        public void Localize()
        {
            if (null != _dropdown)
            {
                _dropdown.ClearOptions();
                List<string> values = new();
                foreach (var key in Keys)
                {
                    values.Add(LocalizationMgr.Instance.Get(key));
                }
                _dropdown.AddOptions(values);
            }
        }

        private void OnDestroy()
        {
            LocalizationMgr.Instance.OnLanguageChanged += Localize;
        }
    }
}