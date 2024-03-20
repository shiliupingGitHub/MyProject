using Game.Script.Game;
using Mirror;

namespace Game.Script.Level
{
    public class FightLevel : Level
    {
        public override void Enter()
        {
            base.Enter();
            switch (GameInstance.Instance.Mode)
            {
                case GameMode.Host:
                {
                    NetworkManager.singleton.StartHost();
                }
                break;
                case GameMode.Client:
                    NetworkManager.singleton.StartClient();
                    break;
            }
        }
    }
}