using System.Collections.Generic;
using Game.Script.Attribute;
using Game.Script.Character.Skill;
using Game.Script.Map;

namespace Game.Script.Subsystem
{
    public class MapEventSubsystem : GameSubsystem
    {
        public Dictionary<MapActionType, MapAction> DefaultActions { get; } = new();
        public Dictionary<MapActionType, System.Type> ActionTypes { get; } = new();
        public override void OnInitialize()
        {
            base.OnInitialize();
            
            DefaultActions.Clear();
            ActionTypes.Clear();
            var baseType = typeof(MapAction);
            var types = baseType.Assembly.GetTypes();

            foreach (var type in types)
            {
                if (baseType.IsAssignableFrom(type) && baseType != type)
                {
                    var attrs = type.GetCustomAttributes(typeof(MapActionDesAttribute), false);

                    foreach (var attr in attrs)
                    {
                        if (attr is MapActionDesAttribute desAttribute)
                        {
                            ActionTypes.Add(desAttribute.ActionType, type);
                            DefaultActions.Add(desAttribute.ActionType, System.Activator.CreateInstance(type) as MapAction);
                        }
                    }
                }
            }
        }
    }
}