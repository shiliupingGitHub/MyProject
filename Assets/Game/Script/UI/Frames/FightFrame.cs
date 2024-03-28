
using Game.Script.Attribute;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Frames
{
    public class FightFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/FightFrame.prefab";

        [UIPath("offset/btnTestPath")] private Button _btnTestPath;

        private LineRenderer _lineRenderer;
        public override void Init(Transform parent)
        {
            base.Init(parent);
            _btnTestPath.onClick.AddListener(() =>
            {
                
            });
        }
    }
}