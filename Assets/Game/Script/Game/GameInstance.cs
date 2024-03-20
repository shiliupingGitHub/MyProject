using System.Collections.Generic;
using System.Reflection;
using Game.Script.Character;
using Game.Script.Common;
using UnityEngine;

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
        private List<BaseController> _controllers = new();

        public void RegisterController(BaseController controller)
        {
            if (!_controllers.Contains(controller))
            {
                _controllers.Add(controller);
            }
        }

        public void Tick()
        {
            foreach (var controller in _controllers)
            {
                controller.Tick(Time.unscaledDeltaTime);
            }
        }

        public void UnRegisterController(BaseController controller)
        {
            _controllers.Remove(controller);
        }

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