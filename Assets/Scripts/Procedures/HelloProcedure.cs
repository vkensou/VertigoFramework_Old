using UnityEngine;
using UML;
using UniRx;

public class HelloProcedure : SimpleProcedure
{
    public HelloProcedure()
        : base("Hello")
    {

    }

    public override ISystemEventRoute EventRoute => null;

    public override StateMachine ProcedureStateMachine => null;

    public override Blackboard Blackboard => null;
}
