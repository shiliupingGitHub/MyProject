using System.Collections.Generic;
using CSVHelper;
using Game.Script.Common;
using Mirror;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Script.Res
{
    public class GameResMgr : Singleton<GameResMgr>
    {

        public void Init()
        {
           var op = Addressables.InitializeAsync();
           op.WaitForCompletion();
            CsvHelper.mLoader += OnCsvRead;
            NetworkClient.OnSpawnHook += OnSpawnNetGo;
        }

        GameObject OnSpawnNetGo(SpawnMessage message)
        {
            var template = LoadAssetSync<GameObject>(message.assetPath);

            if (null != template)
            {
               var go=  GameObject.Instantiate(template);
               return go;
            }
            return null;
        }
        public void OnCsvRead(string szName, System.Action<string, string, System.Action<List<CsvRow>>> readCallBack, System.Action<List<CsvRow>> userCallBack)
        {
            var path = System.IO.Path.Combine("Assets/Game/Res/Config/" , szName + ".csv");
            var op =  Addressables.LoadAssetAsync<TextAsset>(path);
            var textAsset = op.WaitForCompletion();

            var content = System.Text.Encoding.GetEncoding("GBK").GetString(textAsset.bytes);
            readCallBack(szName, content, userCallBack);
        }

        public T LoadAssetSync<T>(string path)
        {
            var op = Addressables.LoadAssetAsync<T>(path);

            return op.WaitForCompletion();
        }
    }
}