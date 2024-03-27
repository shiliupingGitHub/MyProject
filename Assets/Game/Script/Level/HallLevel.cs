using Game.Script.UI;
using Game.Script.UI.Frames;
using UnityEngine.SceneManagement;

namespace Game.Script.Level
{
    public class HallLevel : Level
    {
        private const string SceneName = "Hall";
        public override void Enter()
        {
            base.Enter();

            SceneManager.LoadScene(SceneName);
            UIManager.Instance.Hide<LoadingFrame>();
            UIManager.Instance.Show<HallFrame>();
        }
    }
}