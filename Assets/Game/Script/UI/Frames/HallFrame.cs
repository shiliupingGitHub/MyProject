using Game.Script.Game;
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
            FrameGo.transform.Find("offset/btnFight").GetComponent<Button>().onClick.AddListener(() =>
            {
                Game.GameInstance.Instance.Mode = GameMode.Host;
                LevelManager.Instance.Enter(LevelType.Fight);
            });
            FrameGo.transform.Find("offset/btnJoin").GetComponent<Button>().onClick.AddListener(() =>
            {
                Game.GameInstance.Instance.Mode = GameMode.Client;
                LevelManager.Instance.Enter(LevelType.Fight);
            });
            FrameGo.transform.Find("offset/btnEdit").GetComponent<Button>().onClick.AddListener(() =>
            {
                Game.GameInstance.Instance.Mode = GameMode.Edit;
                LevelManager.Instance.Enter(LevelType.Edit);
            });
        }
    }
}