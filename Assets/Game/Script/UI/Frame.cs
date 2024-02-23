using Castle.Core.Internal;
using Game.Script.Res;
using UnityEngine;

namespace Game.Script.UI
{
    public class Frame
    {
        private GameObject _gameObject;

        public GameObject FrameGo => _gameObject;

        protected virtual string ResPath => string.Empty;

        public virtual void Init(Transform parent)
        {
            if (!ResPath.IsNullOrEmpty())
            {
                var asset = GameResMgr.Instance.LoadAssetSync<GameObject>(ResPath);
                _gameObject = UnityEngine.Object.Instantiate(asset,parent);
            }
           
        }
    }
}