using Entitas;
using UML;

public class SystemSwitchStateEvent : ISystemEvent
{
    public string state;
    public StateEventArg arg;
}

public class SystemStateTransitionEvent : ISystemEvent
{
    public string transition;
    public StateEventArg eventArg;
}
