using Game.Script.Common;
using UnityEditor;

namespace Game.Script.Setting
{
    [InitializeOnLoad]
    public static class GameSettingInitializer
    {
        static GameSettingInitializer()
        {
            GameSetting.Instance.Init();
        }
    }
    public class GameSetting : Singleton<GameSetting>
    {
        public bool ShowGrid { get; set; }
        public void Init()
        {
            ShowGrid = false;
        }
    }
}