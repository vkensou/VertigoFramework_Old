using UnityEngine;
using System.Collections;

public interface IProcedure
{
    ISystemEventRoute EventRoute { get; }
    UML.StateMachine ProcedureStateMachine { get; }
}
