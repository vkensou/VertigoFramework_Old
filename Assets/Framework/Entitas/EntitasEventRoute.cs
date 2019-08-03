using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Assertions;

public class EntitasEventRoute : IEventRoute<GameEntity>
{
    readonly GameContext context;
    readonly Entitas.PrimaryEntityIndex<GameEntity, int> entityGetter;
    Dictionary<System.Type, ISystemEvent> events = new Dictionary<System.Type, ISystemEvent>();

    class EntityEventPool
    {
        public LinkedList<IEntityEvent> events = new LinkedList<IEntityEvent>();
        public LinkedList<IEntityEvent> cachedEvents = new LinkedList<IEntityEvent>();
        public LinkedList<IEntityEvent> cachedNodes = new LinkedList<IEntityEvent>();
    }

    class EntityEventPool<E> : EntityEventPool where E : IEntityEvent
    {
        public static EntityEventPool<E> Instance;
    }

    Dictionary<System.Type, EntityEventPool> entityEvents = new Dictionary<System.Type, EntityEventPool>();

    public EntitasEventRoute(Contexts contexts)
    {
        this.context = contexts.game;
        entityGetter = ((Entitas.PrimaryEntityIndex<GameEntity, int>)context.GetEntityIndex(Contexts.EventHandler));
    }

    public void SendEvent<E>(EventSendType sendType, E _event) where E : ISystemEvent
    {
        if (CheckEvent<E>())
            return;

        _event.SendType = sendType;
        events.Add(typeof(E), _event);
    }

    public E TakeEvent<E>() where E : ISystemEvent
    {
        if (!CheckEvent<E>())
            return default(E);

        E e = (E)events[typeof(E)];
        if (e.SendType == EventSendType.OneTime)
            events.Remove(typeof(E));
        return e;
    }

    public bool TryTakeEvent<E>(out E _event) where E : ISystemEvent
    {
        if (CheckEvent<E>())
        {
            _event = (E)events[typeof(E)];
            if (_event.SendType == EventSendType.OneTime)
                events.Remove(typeof(E));
            return true;
        }

        _event = default(E);
        return false;
    }

    public bool TryTakeEvent<E>() where E : ISystemEvent
    {
        if (CheckEvent<E>())
        {
            E _event = (E)events[typeof(E)];
            if (_event.SendType == EventSendType.OneTime)
                events.Remove(typeof(E));
            return true;
        }

        return false;
    }

    public bool CheckEvent<E>() where E : ISystemEvent
    {
        return events.ContainsKey(typeof(E));
    }

    public void RemoveEvent<E>() where E : ISystemEvent
    {
        if (CheckEvent<E>())
            events.Remove(typeof(E));
    }

    public void Dispose()
    {
        RemoveAll();
    }

    public void RemoveAll()
    {
        events.Clear();
        entityEvents.Clear();
    }

    public void ClearOutOfDateEvents()
    {
        events.Clear();
        foreach (var eventListIter in entityEvents)
        {
            var pool = eventListIter.Value;
            var eventList = pool.events;
            if (eventList.Count == 0)
                continue;
            var iter = eventList.First;
            while (iter != null)
            {
                if (true)
                {
                    var tempNode = iter.Next;
                    eventList.Remove(iter);
                    pool.cachedEvents.AddLast(iter);
                    iter = tempNode;
                }
                else
                {
                    iter = iter.Next;
                }
            }
            eventList.Clear();
        }
    }

    private EntityEventPool<E> GetEntityEventPool<E>() where E :IEntityEvent
    {
        EntityEventPool<E> pool = EntityEventPool<E>.Instance;
        if (pool == null)
        {
            EntityEventPool<E>.Instance = pool = new EntityEventPool<E>();
            entityEvents.Add(typeof(E), pool);
        }
        return pool;
    }

    public void SendEntityEvent<E>(int entityId, EventSendType sendType, E _event) where E : IEntityEvent
    {
        var pool = GetEntityEventPool<E>();
        _event.SendType = sendType;
        _event.entityId = entityId;
        if (pool.cachedNodes.Count > 0)
        {
            var node = pool.cachedNodes.Last;
            pool.cachedNodes.Remove(node);
            node.Value = _event;
            pool.events.AddLast(node);
        }
        else
            pool.events.AddLast(_event);
    }

    public void HandleEntityEvents<E>(IEntityEventHandler<GameEntity, E> handler) where E : IEntityEvent
    {
        Assert.IsNotNull(handler);

        var pool = GetEntityEventPool<E>();
        var eventList = pool.events;
        var iter = eventList.First;
        while (iter != null)
        {
            bool needDelete = true;
            var entityEvent = iter.Value;

            if (entityEvent != null)
            {
                var entity = entityGetter.GetEntity(entityEvent.entityId);
                if (entity != null)
                {
                    if (handler.Filter(entity))
                        handler.HandleEvent(entity, entityEvent as E);
                    if (entityEvent.SendType != EventSendType.OneTime)
                        needDelete = false;
                }
            }

            if (needDelete)
            {
                var tempNode = iter.Next;
                eventList.Remove(iter);
                pool.cachedEvents.AddLast(iter);
                iter = tempNode;
            }
            else
                iter = iter.Next;
        }
    }

    public E GetEntityEvent<E>() where E : IEntityEvent, new()
    {
        var pool = GetEntityEventPool<E>();
        if (pool.cachedEvents.Count > 0)
        {
            var node = pool.cachedEvents.Last;
            E _event = node.Value as E;
            node.Value = null;
            pool.cachedEvents.Remove(node);
            pool.cachedNodes.AddLast(node);
            return _event;
        }
        return new E();
    }
}
