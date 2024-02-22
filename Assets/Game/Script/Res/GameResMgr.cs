using System.Collections.Generic;
using CSVHelper;
using Game.Script.Common;
using UnityEngine;

namespace Game.Script.Res
{
    public class GameResMgr : Singleton<GameResMgr>
    {

        public void OnCsvRead(string szName, System.Action<string, string, System.Action<List<CsvRow>>> readCallBack, System.Action<List<CsvRow>> userCallBack)
        {
#if UNITY_EDITOR
            var path = System.IO.Path.Combine("Assets/Game/Res/Config/" , szName + ".csv");
            var textAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path);

            readCallBack(szName, textAsset.text, userCallBack);
#endif
        }
    }
}