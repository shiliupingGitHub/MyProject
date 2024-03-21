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
        public System.Action<LevelType, LevelType> preLevelChange;
        private readonly Dictionary<LevelType, Level> _levels = new Dictionary<LevelType, Level>()
        {
            {LevelType.Hall, new HallLevel()},
            { LevelType.Fight , new FightLevel()},
            { LevelType.Edit , new EditLevel()},
        };

        private LevelType _curLevel = LevelType.None;
        
        public void Enter(LevelType levelType)
        {
            if (preLevelChange != null)
            {
                preLevelChange.Invoke(_curLevel, levelType);
            }
            if (_levels.TryGetValue(_curLevel, out var level))
            {
                level.Leave();
            }
            UIManager.Instance.Clear();
            _levels[levelType].Enter();
            _curLevel = levelType;
        }
    }
}