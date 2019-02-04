using UnityEngine;
using System.Collections;
using System;

public interface ISystemEventRoute : IDisposable
{
    void SendEvent<E>(EventSendType sendType, E _event) where E : ISystemEvent;
    E TakeEvent<E>() where E : ISystemEvent;
    bool TryTakeEvent<E>(out E _event) where E : ISystemEvent;

    bool CheckEvent<E>() where E : ISystemEvent;
    void RemoveEvent<E>() where E : ISystemEvent;

    void RemoveAll();
}
