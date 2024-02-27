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
        }
    }
}