using System.Collections.Generic;
using Game.Script.Common;
using Game.Script.Res;
using UnityEngine;

namespace Game.Script.UI
{
    public class UIManager : Singleton<UIManager>
    {
        private GameObject _uiRoot;
        private bool bInit;
        private List<Frame> queueFrames = new List<Frame>();
        private Transform _baseRoot;
        private Transform _topRoot;
        public void Init()
        {
            if (!bInit)
            {
                var rootTemplate = GameResMgr.Instance.LoadAssetSync<GameObject>("Assets/Game/Res/UI/UIRoot.prefab");
                _uiRoot = Object.Instantiate(rootTemplate);
                Object.DontDestroyOnLoad(_uiRoot);
                _baseRoot = _uiRoot.transform.Find("Canvas/base");
                _topRoot = _uiRoot.transform.Find("Canvas/top");
                bInit = true;
            }
        }

        public void Clear()
        {
            foreach (var frame in queueFrames)
            {
                frame.Destroy();
              
            }
            queueFrames.Clear();
        }

        public void Hide<T>() where T : Frame
        {
            T curFrame = null;
            foreach (var frame in queueFrames)
            {
                if (frame.GetType() == typeof(T))
                {
                    curFrame = frame as T;
                    break;
                }
            }

            if (curFrame != null)
            {
                curFrame.Hide();
            }
        }
        
        public T Show<T>(bool bUseQueue = true, bool top = false) where  T : Frame
        {
            T ret = null;
            if (bUseQueue)
            {
                foreach (var frame in queueFrames)
                {
                    if (frame.GetType() == typeof(T))
                    {
                        ret = frame as T;
                        queueFrames.Remove(frame);
                        break;
                    }
                }
            }

            if (ret == null)
            {
                ret = System.Activator.CreateInstance<T>();
            
                ret.Init(top?_topRoot:_baseRoot);
            }
            
            ret.FrameGo.transform.SetAsLastSibling();
            queueFrames.Add(ret);
            ret.Show();
            
            return ret;
        }
    }
}