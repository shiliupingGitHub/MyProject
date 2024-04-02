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

            var mapSubsystem = Common.Game.Instance.GetSubsystem<MapSubsystem>();

            var bornPosition = mapSubsystem.GetRandomBornPosition();
            GameObject player = Instantiate(playerPrefab, bornPosition, quaternion.identity);

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
                Common.Game.Instance.serverFightSceneChanged?.Invoke();
            }
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            var levelSystem = Common.Game.Instance.GetSubsystem<LevelSubsystem>();
            levelSystem.Enter(LevelType.Hall);
            
        }
    }
}