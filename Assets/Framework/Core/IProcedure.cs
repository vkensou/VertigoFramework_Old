using UnityEngine;
using System.Collections;

public interface IProcedure
{
    IEventRoute EventRoute { get; }
    UML.StateMachine ProcedureStateMachine { get; }
}
