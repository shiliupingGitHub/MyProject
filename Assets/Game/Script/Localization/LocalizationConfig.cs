using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Script.UI.Extern
{
    public class LocalizationConfig : MonoBehaviour
    {
        public List<SystemLanguage> _keys = new();
        public List<TextAsset> _values = new();

        //Unity doesn't know how to serialize a Dictionary
        public Dictionary<SystemLanguage, TextAsset>  _myDictionary = new();
        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (var kvp in _myDictionary)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            _myDictionary = new();

            for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
                _myDictionary.Add(_keys[i], _values[i]);
        }
    }
}