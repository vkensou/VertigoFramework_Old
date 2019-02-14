using UnityEngine;
using System.Collections;
using UML;

public class ProcedurePauseEvent : StateEventArg
{
    public bool rootNodeHide = false;

    public static ProcedurePauseEvent Create(bool rootNodeHide)
    {
        var e = new ProcedurePauseEvent();
        e.rootNodeHide = rootNodeHide;

        return e;
    }
}
