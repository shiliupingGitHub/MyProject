using System;
using Game.Script.Res;
using UnityEngine;

namespace Game.Script.Entry
{
    public class Entry : MonoBehaviour
    {
        private void Start()
        {
            CSVHelper.CsvHelper.mLoader += GameResMgr.Instance.OnCsvRead;
        }
    }
}