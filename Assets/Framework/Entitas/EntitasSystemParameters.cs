using UML;

public class EntitasSystemEnvironment : SystemEnvironment
{
    public GameContext Context { get; }

    public EntitasSystemEnvironment(GameContext context, IEventRoute eventRoute, Blackboard blackboard, StateMachine stateMachine, Schedulable schedulable)
        : base(eventRoute, blackboard, stateMachine, schedulable)

    {
        Context = context;
    }
}
