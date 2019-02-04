using System;

namespace UML
{
    public enum StateKind
    {
        Simple,
        Composite,
        Submachine
    }

    public class EnterEventArg
    {

    }

    public abstract class State : Vertex
    {
        public StateKind Kind { get; }

        public event Action<EnterEventArg> EntryEvent;
        public event Action ExitEvent;
        public event Action UpdateEvent;
        public event Action InitialEvent;
        public event Action DestroyEvent;
        public Func<bool> Invariant;

        public State(string name)
            :base(name)
        {
        }

        internal virtual bool StateInvariant { get { if (Invariant == null) return true; else return Invariant(); } }

        protected override StateMachine GetContainningStateMachine()
        {
            return null;
        }

        protected virtual void OnEnter(EnterEventArg arg)
        {
            EntryEvent?.Invoke(arg);
        }

        protected virtual void OnLeave()
        {
            ExitEvent?.Invoke();
        }

        protected virtual void OnUpdate()
        {
            UpdateEvent?.Invoke();
        }

        internal virtual void EnterImmediate(EnterEventArg arg)
        {
            Container.EnterChild(this, arg);
            OnEnter(arg);
        }

        internal virtual void EnterFromParent(EnterEventArg arg)
        {
            OnEnter(arg);
        }

        internal virtual void EnterFromChild(Region r, EnterEventArg arg)
        {
            Container.EnterChild(this, arg);
            OnEnter(arg);
        }

        internal virtual void LeaveImmediate(Region lca)
        {
            OnLeave();
            Container.LeaveChild(this, lca);
        }

        internal virtual void LeaveFromParent()
        {
            OnLeave();
        }

        internal void LeaveFromChild(Region lca)
        {
            OnLeave();
            Container.LeaveChild(this, lca);
        }

        internal virtual void Update()
        {
            OnUpdate();
        }

        internal virtual void Initialize()
        {
            InitialEvent?.Invoke();
        }

        internal virtual void Destroy()
        {
            DestroyEvent?.Invoke();
        }

        internal virtual bool HandleEvent(string transitionName, EnterEventArg arg)
        {
            return false;
        }
    }
}

