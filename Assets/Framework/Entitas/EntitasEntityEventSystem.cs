using Entitas;

public abstract class EntitasEntityEventSystem<EventType> : EntitasSystemBase, IEntityEventHandler<GameEntity, EventType> where EventType : IEntityEvent
{
    public EntitasEntityEventSystem(EntitasSystemEnvironment parameters)
        : base(parameters)
    {
    }

    public abstract bool Filter(GameEntity entity);

    public override void Execute()
    {
        (EventRoute as IEventRoute<GameEntity>).HandleEntityEvents(this);
    }

    public abstract void HandleEvent(GameEntity entity, EventType entityEvent);
}
