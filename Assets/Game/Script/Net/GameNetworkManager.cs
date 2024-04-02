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

            var player = Common.Game.Instance.serverFightNewPlayer.Invoke();
            
            player.name = $"{player.name} [connId={conn.connectionId}]";
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