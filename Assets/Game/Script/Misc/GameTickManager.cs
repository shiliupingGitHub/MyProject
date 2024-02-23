using UnityEngine;

namespace Game.Script.Misc
{
    public class GameTickManager : MonoBehaviour
    {
        private static GameTickManager _instance;

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
        public void Init()
        {
            
        }
    }
}