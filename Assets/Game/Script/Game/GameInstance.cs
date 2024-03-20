using System.Collections.Generic;
using System.Reflection;
using Game.Script.Common;

namespace Game.Script.Game
{
    public enum GameMode
    {
        Hall,
        Host,
        Edit,
        Client,
    }
    public class GameInstance : SingletonWithOnInstance<GameInstance>
    {
        public GameMode Mode { set; get; } = GameMode.Host;

        private List<GameSubsystem> _subsystems;

        public override void OnOnInstance()
        {
            base.OnOnInstance();
            var baseType = typeof(GameSubsystem);
            var assem = baseType.Assembly;
            foreach (var type in assem.GetTypes())
            {
                if (baseType.IsAssignableFrom(type) && type != baseType)
                {
                    if (System.Activator.CreateInstance(type) is GameSubsystem subsystem)
                    {
                        subsystem.OnInitialize();
                        _subsystems.Add(subsystem);
                    }
                   
                }
            }
          
        }
    }
}