
using System.Reflection;
using Game.Script.Attribute;
using Game.Script.Map;
using Game.Script.Map.Actor;

using Game.Script.UI.Ext;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Script.UI.Frames
{
    public class ActorDataEditFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/ActorDataEditFrame.prefab";
        [UIPath("offset/btnClose")] private Button _btnClose;
        [UIPath("offset/params")] private Transform _paramsRoot;
        [UIPath("offset/floatParam")] private GameObject _floatParam;
        [UIPath("offset/intParam")] private GameObject _intParam;
        [UIPath("offset/stringParam")] private GameObject _stringParam;
        private ActorData _curActorData;
    

        public override void Init(Transform parent)
        {
            base.Init(parent);
            _btnClose.onClick.AddListener(Hide);
        }
        public void SetActorData(ActorData actorData)
        {
            _curActorData = actorData;
            RefreshActorUI();
        }

        void RefreshActorUI()
        {
            for (int i = _paramsRoot.childCount - 1; i >= 0; --i)
            {
                Object.Destroy(_paramsRoot.GetChild(i).gameObject);
            }

            if (_curActorData.go == null)
            {
                return;
            }

            var mapActor = _curActorData.go.GetComponent<MapActor>();

            if (null == mapActor)
            {
                return;
            }
            
           
            var typeInfo = (TypeInfo)mapActor.GetType();
            
            foreach (var field in typeInfo.DeclaredFields)
            {
                if (field.IsStatic)
                {
                    continue;
                }

                if (!field.IsPublic)
                {
                    continue;
                }
                
                var attr = field.GetCustomAttribute<ActorDataDesAttribute>();

                if (null != attr)
                {
                    FieldDrawer.Draw(_paramsRoot, field, mapActor, o =>
                    {
                        _curActorData.Set(field.Name, o);
                    });
                }
              
                
            }
        }
    }
}