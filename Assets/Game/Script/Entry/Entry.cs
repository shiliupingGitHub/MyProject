using System;
using Game.Script.Level;
using Game.Script.Res;
using Game.Script.UI;
using UnityEngine;

namespace Game.Script.Entry
{
    public class Entry : MonoBehaviour
    {
        private void Start()
        {
            CSVHelper.CsvHelper.mLoader += GameResMgr.Instance.OnCsvRead;
            UIManager.Instance.Init();
            LevelManager.Instance.Enter(LevelType.Hall);
        }
    }
}