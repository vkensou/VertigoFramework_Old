using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : GameControllerBase
{
    protected override IProcedureManager InitialProcedureManager()
    {
        var procedureManager = new EntitasProcedureManager();
        var hello = new HelloProcedure();
        procedureManager.AddProcedure(hello);

        procedureManager.SetInitialProcedure(hello);
        return procedureManager;
    }
}
