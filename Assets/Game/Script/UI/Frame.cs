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

        public virtual void Destroy()
        {
            if (null != FrameGo)
            {
                Object.Destroy(FrameGo);
                _gameObject = null;
            }
            OnDestroy();
        }

        public void Show()
        {
            if (null != FrameGo)
            {
                FrameGo.SetActive(true);
            }
            OnShow();
        }

        public void Hide()
        {
            if (null != FrameGo)
            {
                FrameGo.SetActive(false);
            }
            OnHide();
        }

        protected virtual void OnShow()
        {
          
        }

        protected virtual void OnHide()
        {
            
        }

        protected virtual void OnDestroy()
        {
            
        }
        
    }
}