using Game.Script.Common;
using Game.Script.Res;
using UnityEngine;

namespace Game.Script.UI
{
    public class UIManager : Singleton<UIManager>
    {
        private GameObject UIRoot = null;
        public void Init()
        {
            var rootTemplate = GameResMgr.Instance.LoadAssetSync<GameObject>("Assets/Game/Res/UI/UIRoot.prefab");

            UIRoot = GameObject.Instantiate(rootTemplate);
            GameObject.DontDestroyOnLoad(UIRoot);
        }
    }
}