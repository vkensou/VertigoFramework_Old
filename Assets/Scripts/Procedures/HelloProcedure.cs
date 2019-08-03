using UnityEngine;
using UML;
using UniRx;

public class HelloProcedure : SimpleProcedure
{
    public HelloProcedure()
        : base("Hello")
    {

    }

    protected override void EnterProcedure(StateEventArg arg)
    {
        Debug.Log("Hello");
    }
    public override IEventRoute EventRoute => null;

    public override StateMachine ProcedureStateMachine => null;

    public override Blackboard Blackboard => null;
}
