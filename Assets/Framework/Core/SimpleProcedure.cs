using UnityEngine;
using System.Collections;

public abstract class SimpleProcedure : UML.State, IProcedure
{
    public SimpleProcedure(string name)
        :base(name)
    {
    }

    public abstract ISystemEventRoute EventRoute { get; }
    public abstract UML.StateMachine ProcedureStateMachine { get; }
    public abstract Blackboard Blackboard { get; }
}
