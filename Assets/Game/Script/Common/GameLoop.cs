using System;
using UnityEngine;

namespace Game.Script.Common
{
    
    public class GameLoop : UnitySingleton<GameLoop>
    {
        public System.Action<float> doUpdate;
        public System.Action<float> doFixedUpdate;
        private System.Action _gameAction;
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

            lock (this)
            {
                if (null != _gameAction)
                {
                    _gameAction.Invoke();
                }

                _gameAction = null;
            }
        }

        private void FixedUpdate()
        {
            if (null != doFixedUpdate)
            {
                doFixedUpdate.Invoke(Time.fixedUnscaledDeltaTime);
            }
        }

        public void RunGameThead(System.Action action)
        {
            lock (this)
            {
                _gameAction += action;
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
        
        public static void AddFixed(System.Action<float> action)
        {
            if (_instance)
            {
                _instance.doFixedUpdate += action;
            }
        }

        public static void RemoveFixed(System.Action<float> action)
        {
            if (_instance)
            {
                _instance.doFixedUpdate -= action;
            }
        }

        
    }
}