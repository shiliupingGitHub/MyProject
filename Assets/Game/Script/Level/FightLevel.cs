using Game.Script.Game;
using Game.Script.Res;
using Game.Script.UI;
using Game.Script.UI.Frames;
using Mirror;
using UnityEngine;

namespace Game.Script.Level
{
    public class FightLevel : Level
    {
       
      
        public override void Enter()
        {
            base.Enter();
            switch (Game.Game.Instance.Mode)
            {
                case GameMode.Host:
                {
                    NetworkManager.singleton.StartHost();
                }
                break;
                case GameMode.Client:
                {
                    NetworkManager.singleton.StartClient();
                }
                break;
            }

            UIManager.Instance.Show<FightFrame>();
        }
    }
}