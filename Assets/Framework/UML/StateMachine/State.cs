using System;

namespace UML
{
    public enum StateKind
    {
        Simple,
        Composite,
        Submachine
    }

    public class StateEventArg
    {

    }

    public abstract class State : Vertex
    {
        public StateKind Kind { get; }

        public event Action<StateEventArg> EntryEvent;
        public event Action<StateEventArg> ExitEvent;
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

        protected virtual void OnEnter(StateEventArg arg)
        {
            EntryEvent?.Invoke(arg);
        }

        protected virtual void OnLeave(StateEventArg arg)
        {
            ExitEvent?.Invoke(arg);
        }

        protected virtual void OnUpdate()
        {
            UpdateEvent?.Invoke();
        }

        internal virtual void EnterImmediate(StateEventArg arg)
        {
            Container.EnterChild(this, arg);
            OnEnter(arg);
        }

        internal virtual void EnterFromParent(StateEventArg arg)
        {
            OnEnter(arg);
        }

        internal virtual void EnterFromChild(Region r, StateEventArg arg)
        {
            Container.EnterChild(this, arg);
            OnEnter(arg);
        }

        internal virtual void LeaveImmediate(Region lca, StateEventArg arg)
        {
            OnLeave(arg);
            Container.LeaveChild(this, lca, arg);
        }

        internal virtual void LeaveFromParent(StateEventArg arg)
        {
            OnLeave(arg);
        }

        internal void LeaveFromChild(Region lca, StateEventArg arg)
        {
            OnLeave(arg);
            Container.LeaveChild(this, lca, arg);
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

        internal virtual bool HandleEvent(string transitionName, StateEventArg arg)
        {
            return false;
        }
    }
}

