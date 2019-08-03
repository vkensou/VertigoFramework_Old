using Entitas;

public abstract class EntitasSystemBase : SystemBase, IInitializeSystem, IExecuteSystem, ITearDownSystem
{
    protected EntitasSystemEnvironment SystemEnvironment { get; }
    protected GameContext Context => SystemEnvironment.Context;

    public EntitasSystemBase(EntitasSystemEnvironment parameters)
        : base(parameters)
    {
        SystemEnvironment = parameters;
    }

    public virtual void Initialize()
    {
    }

    public virtual void Execute()
    {
    }

    public virtual void TearDown()
    {
    }
}
