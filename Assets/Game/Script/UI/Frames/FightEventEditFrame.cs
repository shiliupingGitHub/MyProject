using Game.Script.Attribute;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Frames
{
    public class FightEventEditFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/FightEventEditFrame.prefab";
        [UIPath("offset/btnClose")] private Button _btnClose;

        public override void Init(Transform parent)
        {
            base.Init(parent);
            _btnClose.onClick.AddListener(Hide);
        }
    }
}