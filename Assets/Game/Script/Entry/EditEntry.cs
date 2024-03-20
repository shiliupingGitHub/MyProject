﻿using Game.Script.Game;
using Game.Script.Level;
using Game.Script.Misc;
using Game.Script.Res;
using Game.Script.UI;
using Game.Script.UI.Extern;
using UnityEngine;

namespace Game.Script.Entry
{
    public class EditEntry : MonoBehaviour
    {
        private void Start()
        {
            GameInstance.Instance.Mode = GameMode.Edit;
            CSVHelper.CsvHelper.mLoader += GameResMgr.Instance.OnCsvRead;
            UIManager.Instance.Init();
            GameTickManager.Instance.Init();
            LocalizationMgr.Instance.Init();
            LevelManager.Instance.Enter(LevelType.Edit);
        }
    }
}