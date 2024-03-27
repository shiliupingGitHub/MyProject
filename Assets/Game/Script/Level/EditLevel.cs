using Game.Script.UI;
using Game.Script.UI.Frames;
using UnityEngine.SceneManagement;

namespace Game.Script.Level
{
    public class EditLevel : Level
    {
        private const string SceneName = "Edit";
        public override void Enter()
        {
            base.Enter();
            SceneManager.LoadScene(SceneName);
            UIManager.Instance.Hide<LoadingFrame>();
            UIManager.Instance.Show<EditFrame>();
        }
    }
}