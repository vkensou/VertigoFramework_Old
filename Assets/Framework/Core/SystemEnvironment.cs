using UML;

public abstract class SystemEnvironment
{
    public IEventRoute EventRoute { get; }
    public Blackboard Blackboard { get; }
    public StateMachine StateMachine { get; }
    public Schedulable Schedulable { get; }

    public SystemEnvironment(IEventRoute eventRoute, Blackboard blackboard, StateMachine stateMachine, Schedulable schedulable)
    {
        this.EventRoute = eventRoute;
        this.Blackboard = blackboard;
        this.StateMachine = stateMachine;
        this.Schedulable = schedulable;
    }
}
