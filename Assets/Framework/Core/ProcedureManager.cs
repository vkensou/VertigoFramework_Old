using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;

public class ProcedureManager : IProcedureManager
{
    public static ProcedureManager SharedInstance { get; private set; }

    private int command;
    private string commandEvent;
    private UML.EnterEventArg commandArg;
    private bool commandShutDown;
    private UML.StateMachine procedureStateMachine = new UML.StateMachine();

    public SimpleProcedure ActiveProcedure
    {
        get
        {
            return (SimpleProcedure)null;
        }
    }

    static public ISystemEventRoute ActiveEventRoute { get { return SharedInstance.ActiveProcedure.EventRoute; } }

    public ProcedureManager()
    {
        Assert.IsNull(SharedInstance);
        SharedInstance = this;
        procedureStateMachine.CreateRegion("Procedures");
    }

    public override void AddProcedure(IProcedure procedure)
    {
        procedureStateMachine.Region.AddState(procedure as UML.State);
    }

    public override void SetInitialProcedure(IProcedure procedure)
    {
        procedureStateMachine.Region.SetInitial(procedure as UML.State);
    }

    public override void AddTransition(string transitionName, IProcedure source, IProcedure target)
    {
        procedureStateMachine.AddTransition(transitionName, source as UML.State, target as UML.State);
    }

    public override void PushCommand(string procedure, UML.EnterEventArg firearg = null)
    {
        if (command == 1)
        {
            Debug.LogError("ProcedureManager has a command!!");
            return;
        }
        command = 1;
        commandEvent = procedure;
        commandArg = firearg;
        commandShutDown = true;
    }

    public override void PushCommand(string procedure, UML.EnterEventArg firearg, bool shutdown)
    {
        if (command == 1)
        {
            Debug.LogError("ProcedureManager has a command!!");
            return;
        }
        command = 1;
        commandEvent = procedure;
        commandArg = firearg;
        commandShutDown = shutdown;
    }

    public override void Update()
    {
        ExecuteCommand();
        procedureStateMachine.Update();
    }

    private void ExecuteCommand()
    {
        if(command == 1)
        {
            procedureStateMachine.FireEvent(commandEvent, commandArg);
        }
        command = 0;
    }

    public override void Start()
    {
        procedureStateMachine.Start();
    }
}

