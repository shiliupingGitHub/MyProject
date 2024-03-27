using Game.Script.Game;
using Game.Script.UI;
using Game.Script.UI.Frames;
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

            UIManager.Instance.Show<FightFrame>();
        }
    }
}