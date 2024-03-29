using System;
using UnityEngine;

namespace Game.Script.Common
{
    
    public class GameLoop : UnitySingleton<GameLoop>
    {
        public System.Action<float> doUpdate;
        [RuntimeInitializeOnLoadMethod]
        static void RuntimeLoad()
        {
            Instance.Init();
        }

        void Init()
        {
            doUpdate = null;
        }
        private void Update()
        {
            
            if (null != doUpdate)
            {
                doUpdate.Invoke(Time.unscaledDeltaTime);
            }
        }

        public static void Add(System.Action<float> action)
        {
            if (_instance)
            {
                _instance.doUpdate += action;
            }
        }

        public static void Remove(System.Action<float> action)
        {
            if (_instance)
            {
                _instance.doUpdate -= action;
            }
        }

        
    }
}