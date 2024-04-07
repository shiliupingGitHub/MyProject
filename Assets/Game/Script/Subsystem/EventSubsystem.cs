using System.Collections.Generic;

namespace Game.Script.Subsystem
{
    public enum EventType
    {
        NONE,
        Fight,
    }
        
    public class EventSubsystem : GameSubsystem
    {
        private Dictionary<string, System.Action<System.Object>> _subscribers = new();
        private List<string> _fightEvents = new();
        public List<string> FightEvents => _fightEvents;
        public void Raise(string eventName, System.Object o = null, EventType eventType = EventType.Fight)
        {
            if(_subscribers.TryGetValue(eventName, out var subscriber))
                subscriber(o);

            if (!_fightEvents.Contains(eventName))
            {
                _fightEvents.Add(eventName);
            }
        }
        public  void Subscribe(string eventName, System.Action<System.Object> subscriber)
        {
            if(_subscribers.ContainsKey(eventName))
                _subscribers[eventName] += subscriber;
            else
            {
                _subscribers[eventName] = subscriber;
            }
        }
        
        public  void UnSubscribe(string eventName, System.Action<System.Object> subscriber)
        {
            if(_subscribers.ContainsKey(eventName))
                _subscribers[eventName] -= subscriber;
        }
    }
}