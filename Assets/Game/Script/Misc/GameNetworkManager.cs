using Game.Script.Map;
using Game.Script.Res;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Script.Misc
{
    public class GameNetworkManager : NetworkManager
    {
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Transform startPos = GetStartPosition();
            GameObject player = startPos != null
                ? Instantiate(playerPrefab, startPos.position, startPos.rotation)
                : Instantiate(playerPrefab, Vector3.zero, quaternion.identity);

            // instantiating a "Player" prefab gives it the name "Player(clone)"
            // => appending the connectionId is WAY more useful for debugging!
            player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
            NetworkServer.AddPlayerForConnection(conn, player);
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);

            if (sceneName.Contains("Fight"))
            {
                MapMgr.Instance.LoadMap("map_test_1", true);
            }
        }
        
    }
}