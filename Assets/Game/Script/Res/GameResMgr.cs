using System.Collections.Generic;
using CSVHelper;
using Game.Script.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Script.Res
{
    public class GameResMgr : Singleton<GameResMgr>
    {

        public void OnCsvRead(string szName, System.Action<string, string, System.Action<List<CsvRow>>> readCallBack, System.Action<List<CsvRow>> userCallBack)
        {
            var path = System.IO.Path.Combine("Assets/Game/Res/Config/" , szName + ".csv");
            var op =  Addressables.LoadAssetAsync<TextAsset>(path);
            var textAsset = op.WaitForCompletion();
            
            readCallBack(szName, textAsset.text, userCallBack);
        }

        public T LoadAssetSync<T>(string path)
        {
            var op = Addressables.LoadAssetAsync<T>(path);

            return op.WaitForCompletion();
        }
    }
}