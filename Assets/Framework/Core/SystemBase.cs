using UnityEngine;
using System.Collections;

public abstract class SystemBase
{
    protected readonly ISystemEventRoute m_eventRoute;
    protected readonly Schedulable schedulable = new Schedulable();

    public SystemBase(ISystemEventRoute eventRoute)
    {
        m_eventRoute = eventRoute;
    }

    protected void SendEvent<E>(EventSendType sendType, E _event) where E : ISystemEvent
    {
        m_eventRoute.SendEvent(sendType, _event);
    }

    protected bool CheckEvent<E>() where E : ISystemEvent
    {
        return m_eventRoute.CheckEvent<E>();
    }

    protected E TakeEvent<E>() where E : ISystemEvent
    {
        return m_eventRoute.TakeEvent<E>();
    }

    protected bool TryTakeEvent<E>(out E _event) where E : ISystemEvent
    {
        return m_eventRoute.TryTakeEvent<E>(out _event);
    }

    protected void ProcessSchedule()
    {
        schedulable.ProcessSchedule(Time.deltaTime);
    }
}
