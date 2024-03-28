
using Game.Script.Character.Skill;
using Game.Script.Game;
using Game.Script.Level;
using Game.Script.Misc;
using Game.Script.UI;
using UnityEngine;
namespace Game.Script.Entry
{
    public class Entry : MonoBehaviour
    {
        public GameMode entryMode  = GameMode.Hall;
        private void Start()
        {
            Game.Game.Instance.Mode = entryMode;
            UIManager.Instance.Init();
            var levelSubsystem = Game.Game.Instance.GetSubsystem<LevelSubsystem>();
            switch (entryMode)
            {
                case GameMode.Hall:
                {
                    levelSubsystem.Enter(LevelType.Hall);
                }
                break;
                case GameMode.Edit:
                    levelSubsystem.Enter(LevelType.Edit);
                    break;
            }
           
         
        }
    }
}