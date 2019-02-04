using Entitas;
using UML;

public class SystemSwitchStateEvent : ISystemEvent
{
    public string state;
    public EnterEventArg arg;
}

public class SystemRequireSwitchStateEvent : ISystemEvent
{
    public string transition;
    public EnterEventArg eventArg;
}
