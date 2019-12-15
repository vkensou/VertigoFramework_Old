using UML;

public abstract class StateBasedSystemBase : SystemBase
{
    protected ISystemState currentState;

    public StateBasedSystemBase(SystemEnvironment systemEnvironment)
        :base(systemEnvironment)
    {
    }

    protected void UpdateState()
    {
        currentState?.Update();
    }

    protected void Shutdown()
    {
        currentState?.Leave();
    }

    protected void SetCurrentState(ISystemState state)
    {
        currentState = state;
    }

    protected abstract void HandleStateEvents();

    protected abstract ISystemState HandleSwtichStateEvent(string state);

    protected abstract class ISystemState
    {
        protected EntitasSystemEnvironment SystemEnvironment { get; set; }
        public ISystemState(EntitasSystemEnvironment systemEnvironment)
        {
            SystemEnvironment = systemEnvironment;
        }

        public virtual void Enter(StateEventArg arg) { }
        public virtual void Update() { }
        public virtual void Leave() { }

        protected void SendEvent<E>(EventSendType sendType, E _event) where E : ISystemEvent
        {
            SystemEnvironment.EventRoute.SendEvent(sendType, _event);
        }

        protected bool CheckEvent<E>() where E : ISystemEvent
        {
            return SystemEnvironment.EventRoute.CheckEvent<E>();
        }

        protected E TakeEvent<E>() where E : ISystemEvent
        {
            return SystemEnvironment.EventRoute.TakeEvent<E>();
        }

        protected bool TryTakeEvent<E>(out E _event) where E : ISystemEvent
        {
            return SystemEnvironment.EventRoute.TryTakeEvent<E>(out _event);
        }

        protected void RequireSwitchState(string transition, StateEventArg arg = null)
        {
            SystemEnvironment.StateMachine.FireEvent(transition, arg);
        }
    }
}
