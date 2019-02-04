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

    public abstract ISystemEventRoute EventRoute { get; }
    public abstract UML.StateMachine ProcedureStateMachine { get; }
}
