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
            if (Application.isPlaying)
            {
                Instance.Init();
            }
            
        }
        void Init()
        {
            
        }
        

        private void Update()
        {
            
            if (null != doUpdate)
            {
                doUpdate.Invoke(Time.unscaledDeltaTime);
            }
        }

        private void OnDestroy()
        {
            doUpdate = null;
            doFixedUpdate = null;
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