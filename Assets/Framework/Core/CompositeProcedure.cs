using UnityEngine;
using System.Collections;
using UML;

public abstract class CompositeProcedure : CompositeState, IProcedure
{
    public CompositeProcedure(string name)
        :base(name)
    {
        CreateRegion(name + "_Region");
    }

    public abstract IEventRoute EventRoute { get; }
    public abstract UML.StateMachine ProcedureStateMachine { get; }

    protected override void OnEnter(StateEventArg arg)
    {
        base.OnEnter(arg);
        EnterProcedure(arg);
    }

    protected override void OnLeave(StateEventArg arg)
    {
        LeaveProcedure(arg);
        base.OnLeave(arg);
    }

    protected virtual void EnterProcedure(StateEventArg arg)
    {

    }

    protected virtual void LeaveProcedure(StateEventArg arg)
    {
    }
}
