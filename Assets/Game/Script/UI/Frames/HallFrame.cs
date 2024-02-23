using Game.Script.Level;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Script.UI.Frames
{
    public class HallFrame : Frame
    {
        protected override string ResPath => "Assets/Game/Res/UI/HallFrame.prefab";
        public override void Init(Transform parent)
        {
            base.Init(parent);
            FrameGo.transform.Find("btnFight").GetComponent<Button>().onClick.AddListener(() =>
            {
                LevelManager.Instance.Enter(LevelType.Fight);
            });
        }
    }
}