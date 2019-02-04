namespace UML
{
    public enum PseudostateKind
    {
        Initial,
        DeepHistory,
        ShallowHistory,
        Join,
        Fork,
        Junction,
        Choice,
        EntryPoint,
        ExitPoint,
        Terminate
    }

    public sealed class Pseudostate : Vertex
    {
        public PseudostateKind Kind { get; } = PseudostateKind.Initial;
        StateMachine stateMachine;

        public Pseudostate(string name)
            :base(name)
        {
        }

        protected override StateMachine GetContainningStateMachine()
        {
            if (Kind == PseudostateKind.EntryPoint || Kind == PseudostateKind.ExitPoint)
                return stateMachine;
            else
                return null;
        }
    }
}

