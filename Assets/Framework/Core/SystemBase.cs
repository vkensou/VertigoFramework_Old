using UML;

public abstract class SystemBase
{
    SystemEnvironment SystemEnvironment { get; }
    protected IEventRoute EventRoute => SystemEnvironment.EventRoute;
    protected Blackboard Blackboard => SystemEnvironment.Blackboard;
    protected StateMachine StateMachine => SystemEnvironment.StateMachine;
    protected Schedulable Schedulable => SystemEnvironment.Schedulable;

    public SystemBase(SystemEnvironment systemEnvironment)
    {
        this.SystemEnvironment = systemEnvironment;
    }

    protected void SendEvent<E>(EventSendType sendType, E _event) where E : ISystemEvent
    {
        EventRoute.SendEvent(sendType, _event);
    }

    protected bool CheckEvent<E>() where E : ISystemEvent
    {
        return EventRoute.CheckEvent<E>();
    }

    protected E TakeEvent<E>() where E : ISystemEvent
    {
        return EventRoute.TakeEvent<E>();
    }

    protected bool TryTakeEvent<E>(out E _event) where E : ISystemEvent
    {
        return EventRoute.TryTakeEvent<E>(out _event);
    }

    protected void SendEntityEvent<E>(int entityId, EventSendType sendType, E _event) where E : IEntityEvent
    {
        EventRoute.SendEntityEvent<E>(entityId, sendType, _event);
    }
}
