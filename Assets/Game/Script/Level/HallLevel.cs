using Game.Script.Attribute;
using Game.Script.Subsystem;
using Game.Script.UI;
using Game.Script.UI.Frames;
using UnityEngine.SceneManagement;

namespace Game.Script.Level
{
    [CustomLevel(LevelType.Hall)]
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