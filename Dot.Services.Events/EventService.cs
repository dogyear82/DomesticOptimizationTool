namespace Dot.Services.Events
{
    public interface IEventService
    {
        void Subscribe(string eventName, Action<object> handler);
        void Unsubscribe(string eventName, Action<object> handler);
        void Emit(string eventName, object eventData = null);
    }

    public class EventService : IEventService
    {
        private readonly Dictionary<string, List<Action<object>>> _eventHandlers = new();

        public void Subscribe(string eventName, Action<object> handler)
        {
            if (!_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName] = new List<Action<object>>();
            }
            _eventHandlers[eventName].Add(handler);
        }

        public void Unsubscribe(string eventName, Action<object> handler)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                _eventHandlers[eventName].Remove(handler);
                if (_eventHandlers[eventName].Count == 0)
                {
                    _eventHandlers.Remove(eventName);
                }
            }
        }

        public void Emit(string eventName, object eventData = null)
        {
            if (_eventHandlers.ContainsKey(eventName))
            {
                foreach (var handler in _eventHandlers[eventName])
                {
                    handler.Invoke(eventData);
                }
            }
        }
    }
}
