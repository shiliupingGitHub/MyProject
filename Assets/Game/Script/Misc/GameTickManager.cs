using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Script.Misc
{
    public class GameTickManager : MonoBehaviour
    {
        private static GameTickManager _instance;

        private List<Action> _actions = new();

        public static GameTickManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("GameTickManager");
                    Object.DontDestroyOnLoad(go);

                    _instance = go.AddComponent<GameTickManager>();

                }

                return _instance;
            }
        }

        private void Update()
        {
            foreach (var action in _actions)
            {
                action.Invoke();
            }
        }

        public void AddTick(System.Action action)
        {
            _actions.Add(action);
        }

        public void RemoveTick(System.Action action)
        {
            _actions.Remove(action);
        }
        public void Init()
        {
            
        }
    }
}