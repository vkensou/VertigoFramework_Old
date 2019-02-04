using UnityEngine;
using System.Collections;

public abstract class StateBasedSystemBase : SystemBase
{
    protected ISystemState currentState;

    public StateBasedSystemBase(ISystemEventRoute eventRoute)
        :base(eventRoute)
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
        protected ISystemEventRoute EventRoute { get; set; }
        public ISystemState(ISystemEventRoute eventRoute)
        {
            EventRoute = eventRoute;
        }

        public virtual void Enter(UML.EnterEventArg arg) { }
        public virtual void Update() { }
        public virtual void Leave() { }

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

        protected abstract void RequireSwitchState(string transition, UML.EnterEventArg arg = null);
    }
}
