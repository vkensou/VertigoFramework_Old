public enum EventSendType { OneTime, OneFrame }

public abstract class ISystemEvent 
{
    public EventSendType SendType;
}

public abstract class IEntityEvent
{
    public EventSendType SendType;
    public int entityId;
}