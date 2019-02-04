using UnityEngine;
using System.Collections.Generic;

public class SystemEventRoute : ISystemEventRoute
{
    Dictionary<System.Type, ISystemEvent> events = new Dictionary<System.Type, ISystemEvent>();

    public void SendEvent<E>(EventSendType sendType, E _event) where E : ISystemEvent
    {
        if (CheckEvent<E>())
            return;

        _event.SendType = sendType;
        events.Add(typeof(E), _event);
    }

    public E TakeEvent<E>() where E : ISystemEvent
    {
        if (!CheckEvent<E>())
            return default(E);

        E e = (E)events[typeof(E)];
        if (e.SendType == EventSendType.OneTime)
            events.Remove(typeof(E));
        return e;
    }

    public bool TryTakeEvent<E>(out E _event) where E : ISystemEvent
    {
        if (CheckEvent<E>())
        {
            _event = (E)events[typeof(E)];
            if (_event.SendType == EventSendType.OneTime)
                events.Remove(typeof(E));
            return true;
        }

        _event = default(E);
        return false;
    }

    public bool CheckEvent<E>() where E : ISystemEvent
    {
        return events.ContainsKey(typeof(E));
    }

    public void RemoveEvent<E>() where E : ISystemEvent
    {
        if (CheckEvent<E>())
            events.Remove(typeof(E));
    }

    public void Dispose()
    {
        RemoveAll();
    }

    public void RemoveAll()
    {
        events.Clear();
    }
}
