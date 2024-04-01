using System.Collections.Generic;
using Game.Script.Level;
using Game.Script.UI;
using Game.Script.UI.Frames;

namespace Game.Script.Subsystem
{
    public enum LevelType
    {
        None,
        Hall,
        Fight,
        Edit,
    }
    public class LevelSubsystem : GameSubsystem
    {
        public System.Action<LevelType, LevelType> preLevelChange;
        private readonly Dictionary<LevelType, Level.Level> _levels = new Dictionary<LevelType, Level.Level>()
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
            UIManager.Instance.Show<LoadingFrame>(true, true);
            _levels[levelType].Enter();
            _curLevel = levelType;
            System.GC.Collect();
        }
    }
}