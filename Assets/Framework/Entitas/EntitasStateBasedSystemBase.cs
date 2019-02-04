using UnityEngine;
using System.Collections;
using Entitas;

public abstract class EntitasStateBasedSystemBase : StateBasedSystemBase, IExecuteSystem
{
    public EntitasStateBasedSystemBase(ISystemEventRoute eventRoute)
        : base(eventRoute)
    {

    }

    public virtual void Execute()
    {
        HandleStateEvents();
        UpdateState();
    }

    protected sealed override void HandleStateEvents()
    {
        SystemSwitchStateEvent stateSwitchEvent;
        if (TryTakeEvent(out stateSwitchEvent))
        {
            currentState?.Leave();
            SetCurrentState(HandleSwtichStateEvent(stateSwitchEvent.state));
            currentState?.Enter(stateSwitchEvent.arg);
        }
    }

    protected abstract class EntitasSystemState : ISystemState
    {
        public EntitasSystemState(ISystemEventRoute eventRoute)
            : base(eventRoute)
        {

        }

        protected override void RequireSwitchState(string transition, UML.EnterEventArg arg = null)
        {
            var e = new SystemRequireSwitchStateEvent{ transition = transition, eventArg = arg };
            EventRoute.SendEvent(EventSendType.OneTime, e);
        }
    }
}