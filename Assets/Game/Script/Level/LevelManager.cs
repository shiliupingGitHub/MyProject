using System.Collections.Generic;
using Game.Script.Common;
using Game.Script.UI;

namespace Game.Script.Level
{
    public enum LevelType
    {
        None,
        Hall,
        Fight,
        Edit,
    }
    public class LevelManager : Singleton<LevelManager>
    {
        
        private Dictionary<LevelType, Level> levels = new Dictionary<LevelType, Level>()
        {
            {LevelType.Hall, new HallLevel()},
            { LevelType.Fight , new FightLevel()},
            { LevelType.Edit , new EditLevel()},
        };

        private LevelType curLevel = LevelType.None;
        
        public void Enter(LevelType levelType)
        {
            if (levels.ContainsKey(curLevel))
            {
                levels[curLevel].Leave();
            }
            UIManager.Instance.Clear();
            levels[levelType].Enter();
            curLevel = levelType;
        }
    }
}