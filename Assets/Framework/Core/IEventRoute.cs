using UnityEngine;
using System.Collections.Generic;
using System;

public interface IEntityEventHandler<EntityType, EventType> where EventType : IEntityEvent
{
    bool Filter(EntityType entity);
    void HandleEvent(EntityType entity, EventType entityEvent);
}

public interface IEventRoute : IDisposable
{
    void SendEvent<E>(EventSendType sendType, E _event) where E : ISystemEvent;
    E TakeEvent<E>() where E : ISystemEvent;
    bool TryTakeEvent<E>(out E _event) where E : ISystemEvent;
    bool TryTakeEvent<E>() where E : ISystemEvent;

    bool CheckEvent<E>() where E : ISystemEvent;
    void RemoveEvent<E>() where E : ISystemEvent;

    void SendEntityEvent<E>(int entityId, EventSendType sendType, E _event) where E : IEntityEvent;
    E GetEntityEvent<E>() where E : IEntityEvent, new();

    void RemoveAll();
    void ClearOutOfDateEvents();
}

public interface IEventRoute<EntityType> : IEventRoute
{
    void HandleEntityEvents<EventType>(IEntityEventHandler<EntityType, EventType> handler) where EventType : IEntityEvent;
}