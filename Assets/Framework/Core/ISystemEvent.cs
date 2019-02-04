public enum EventSendType { OneTime, OneFrame }

public abstract class ISystemEvent 
{
    public EventSendType SendType;
}
