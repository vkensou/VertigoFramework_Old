public abstract class IProcedureManager
{
    IProcedure ActiveProcedure { get; }
    public abstract void AddProcedure(IProcedure procedure);
    public abstract void SetInitialProcedure(IProcedure procedure);
    public abstract void AddTransition(string transitionName, IProcedure source, IProcedure target);
    public abstract void PushCommand(string procedure, UML.StateEventArg firearg = null);
    public abstract void PushCommand(string procedure, UML.StateEventArg firearg, bool shutdown);
    public abstract void Start();
    public abstract void Update();
}
