using Game.Script.Common;

namespace Game.Script.Misc
{
    public enum GameMode
    {
        Game,
        Edit,
    }
    public class GameInstance : Singleton<GameInstance>
    {
        public GameMode Mode { set; get; } = GameMode.Game;
    }
}