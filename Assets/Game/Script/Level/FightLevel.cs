using Mirror;

namespace Game.Script.Level
{
    public class FightLevel : Level
    {
        public override void Enter()
        {
            base.Enter();
            NetworkManager.singleton.StartHost();
        }
    }
}