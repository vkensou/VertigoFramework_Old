using UnityEngine;
using System.Collections;
using UML;

public abstract class SimpleProcedure : UML.State, IProcedure
{
    readonly bool suspendable;
    protected bool Running { get; private set; }
    bool m_paused = false;

    public SimpleProcedure(string name, bool suspendable = false)
        :base(name)
    {
        this.suspendable = suspendable;
    }
    public virtual IEventRoute EventRoute { get; }
    public virtual UML.StateMachine ProcedureStateMachine { get; }
    public virtual Blackboard Blackboard { get; }

    protected override void OnEnter(StateEventArg arg)
    {
        base.OnEnter(arg);
        if (!Running)
        {
            if (suspendable && m_paused)
                ResumeProcedure(arg);
            else
                EnterProcedure(arg);

            m_paused = false;
            Running = true;
        }
    }

    protected override void OnLeave(StateEventArg arg)
    {
        OnLeaveProcedure(arg);
        base.OnLeave(arg);
    }

    protected virtual void EnterProcedure(StateEventArg arg)
    {

    }

    protected virtual void PauseProcedure(ProcedurePauseEvent arg)
    {

    }

    protected virtual void ResumeProcedure(StateEventArg arg)
    {

    }

    protected virtual void LeaveProcedure(StateEventArg arg)
    {
    }

    private void OnLeaveProcedure(StateEventArg arg)
    {
        if (Running)
        {
            if (suspendable && !m_paused && arg is ProcedurePauseEvent)
            {
                m_paused = true;
                PauseProcedure(arg as ProcedurePauseEvent);
            }
            else
            {
                Running = false;
                LeaveProcedure(arg);
            }
        }
    }

    public void ForceLeaveProcedure()
    {
        OnLeaveProcedure(null);
    }
}
