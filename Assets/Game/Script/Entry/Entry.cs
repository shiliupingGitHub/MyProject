using System;
using Game.Script.Character.Skill;
using Game.Script.Game;
using Game.Script.Level;
using Game.Script.Map;
using Game.Script.Misc;
using Game.Script.Res;
using Game.Script.UI;
using Game.Script.UI.Extern;
using UnityEngine;

namespace Game.Script.Entry
{
    public class Entry : MonoBehaviour
    {
        public GameMode EntryMode  = GameMode.Hall;
        private void Start()
        {
            GameResMgr.Instance.Init();
            GameInstance.Instance.Mode = EntryMode;
            UIManager.Instance.Init();
            GameTickManager.Instance.Init();
            SkillMgr.Instance.Init();
            LocalizationMgr.Instance.Init();
            MapMgr.Instance.Init();

            switch (EntryMode)
            {
                case GameMode.Hall:
                {
                    LevelManager.Instance.Enter(LevelType.Hall);
                }
                break;
                case GameMode.Edit:
                    LevelManager.Instance.Enter(LevelType.Edit);
                    break;
            }
           
         
        }
    }
}