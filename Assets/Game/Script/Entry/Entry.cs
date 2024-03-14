using System;
using Game.Script.Character.Skill;
using Game.Script.Game;
using Game.Script.Level;
using Game.Script.Misc;
using Game.Script.Res;
using Game.Script.UI;
using UnityEngine;

namespace Game.Script.Entry
{
    public class Entry : MonoBehaviour
    {
        private void Start()
        {
            GameInstance.Instance.Mode = GameMode.Game;
            CSVHelper.CsvHelper.mLoader += GameResMgr.Instance.OnCsvRead;
            UIManager.Instance.Init();
            GameTickManager.Instance.Init();
            SkillMgr.Instance.Init();
            LevelManager.Instance.Enter(LevelType.Hall);
        }
    }
}