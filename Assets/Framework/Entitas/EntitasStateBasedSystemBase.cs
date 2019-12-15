using UnityEngine;
using System.Collections;
using Entitas;

public abstract class EntitasStateBasedSystemBase : StateBasedSystemBase, IInitializeSystem, IExecuteSystem, ITearDownSystem
{
    EntitasSystemEnvironment systemEnvironment;
    protected GameContext Context => systemEnvironment.Context;

    public EntitasStateBasedSystemBase(EntitasSystemEnvironment parameters)
        : base(parameters)
    {
        systemEnvironment = parameters;
    }

    public virtual void Initialize()
    {
        throw new System.NotImplementedException();
    }

    public virtual void Execute()
    {
        HandleStateEvents();
        UpdateState();
    }

    public void TearDown()
    {
        Shutdown();
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
        public EntitasSystemState(EntitasSystemEnvironment systemEnvironment)
            : base(systemEnvironment)
        {

        }
    }
}