using Game.Script.Subsystem;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

namespace Game.Script.Misc
{
    public class GameNetworkManager : NetworkManager
    {
        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            GameObject player = Instantiate(playerPrefab, new Vector3(3, 1, 0), quaternion.identity);

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
                var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();
                mapSubsystem.LoadMap("map_test_1", true);
            }
        }
    }
}