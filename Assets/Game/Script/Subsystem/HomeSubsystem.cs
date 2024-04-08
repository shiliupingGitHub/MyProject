using System.Collections.Generic;
using Game.Script.Common;
using Game.Script.Home.Actor;

namespace Game.Script.Subsystem
{
    public class HomeSubsystem : GameSubsystem
    {
        private List<HomeActor> _homeActors = new();

        public override void OnInitialize()
        {
            base.OnInitialize();
            GameLoop.AddQuit(SaveAchieve);
        }

        public void LoadAchieve()
        {
            
        }

        public void SaveAchieve()
        {
            
        }
    }
}