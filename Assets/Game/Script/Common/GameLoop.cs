using System;
using UnityEngine;

namespace Game.Script.Common
{
    public class GameLoop : UnitySingleton<GameLoop>
    {
        public System.Action<float> doUpdate;
        public System.Action<float> doFixedUpdate;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void RuntimeLoad()
        {
            Instance.Init();
        }
        void Init()
        {
            
        }
        

        private void Update()
        {
            if (null != doUpdate)
            {
                doUpdate.Invoke(Time.unscaledTime);
            }
        }

        private void FixedUpdate()
        {
            if (null != doFixedUpdate)
            {
                doFixedUpdate.Invoke(Time.unscaledTime);
            }
        }
    }
}