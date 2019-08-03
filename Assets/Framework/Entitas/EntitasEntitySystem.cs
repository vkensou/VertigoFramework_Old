using Entitas;

public abstract class EntitasEntitySystem : EntitasSystemBase
{
    IGroup<GameEntity> entityGroup;

    public EntitasEntitySystem(EntitasSystemEnvironment parameters)
    : base(parameters)
    {
    }

    protected abstract IMatcher<GameEntity> GetMatcher();

    public override void Initialize()
    {
        entityGroup = Context.GetGroup(GetMatcher());
    }

    public override void Execute()
    {
        foreach (var entity in entityGroup)
            ProcessEntity(entity);
    }

    protected abstract void ProcessEntity(GameEntity entity);
}
