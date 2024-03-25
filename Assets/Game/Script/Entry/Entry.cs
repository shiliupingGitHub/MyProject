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
        private void Start()
        {
            GameResMgr.Instance.Init();
            GameInstance.Instance.Mode = GameMode.Hall;
            UIManager.Instance.Init();
            GameTickManager.Instance.Init();
            SkillMgr.Instance.Init();
            LocalizationMgr.Instance.Init();
            MapMgr.Instance.Init();
            LevelManager.Instance.Enter(LevelType.Hall);
         
        }
    }
}