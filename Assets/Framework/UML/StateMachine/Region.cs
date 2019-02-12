using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

namespace UML
{
    public class Region
    {
        public string Name { get; }
        readonly StateMachine stateMachine;
        public State State { get; }
        private State initial;
        private State rootActive;
        private State active;
        List<Vertex> subvertices = new List<Vertex>();
        Dictionary<string, Transition> transitions = new Dictionary<string, Transition>();
        List<Transition> autoTransitions = new List<Transition>();
        public event Action FinalEvent;
        public event Action TerminateEvent;
        public bool IsFinal { get; private set; }
        public bool IsTerminate { get; set; }
        public Region(string name, StateMachine stateMachine)
        {
            Name = name;
            this.stateMachine = stateMachine;
        }
        
        public Region(string name, State state)
        {
            Name = name;
            State = state;
        }

        public StateMachine StateMachine
        {
            get
            {
                if (stateMachine != null)
                    return stateMachine;
                else
                    return State.StateMachine;
            }
        }

        public List<Vertex> Subvertices => subvertices;

        public Region AddState(State state)
        {
            Assert.IsNull(state.Container);

            state.Container = this;
            subvertices.Add(state);

            return this;
        }

        public void AddStates(params State[] states)
        {
            foreach(var state in states)
            {
                AddState(state);
            }
        }

        public void SetInitial(State state)
        {
            Assert.AreEqual(state.Container, this);

            initial = state;
        }

        internal void AddTransitionImpl(string name, Transition t)
        {
            transitions.Add(name, t);
        }

        internal void AddAutoTransitionImpl(Transition t)
        {
            int sourceDepth = StateMachine.VertexDepth(t.Source);
            int index = 0;
            for (int i = 0; i < autoTransitions.Count; ++i)
            {
                var ti = autoTransitions[i];
                int d = StateMachine.VertexDepth(ti.Source);
                if(d > sourceDepth || (t.Guard != null && ti.Guard == null))
                {
                    index = i;
                    break;
                }
            }
            autoTransitions.Insert(index, t);
        }

        public void Enter(StateEventArg arg)
        {
            Assert.IsNotNull(initial);

            ChangeActive(initial, initial, null);
            rootActive.EnterFromParent(arg);
        }

        public void EnterChild(State child, StateEventArg arg)
        {
            Assert.IsNotNull(child);
            Assert.AreEqual(child.Container, this);

            if (rootActive != child)
            {
                if (State != null)
                {
                    State.EnterFromChild(this, arg);
                    ChangeActiveImpl(child, child, null);
                }
            }

        }

        public void LeaveChild(State child, Region lca, StateEventArg arg)
        {
            Assert.IsNotNull(child);
            Assert.AreEqual(child.Container, this);

            if (this != lca)
                if (State != null)
                    State.LeaveFromChild(lca, arg);
        }

        public void Leave(StateEventArg arg)
        {
            Assert.IsNotNull(rootActive);

            rootActive.LeaveFromParent(arg);
            ChangeActiveImpl(null, null, null);
        }

        public void Update()
        {
            Assert.IsNotNull(rootActive);

            rootActive.Update();
            if (!active.StateInvariant)
            {
                foreach(var t in autoTransitions)
                {
                    if (t.Source != active)
                        continue;

                    if (t.Guard != null && !t.Guard())
                        continue;

                    Assert.AreEqual(this, StateMachine.LCA(t.Source, t.Target));

                    rootActive.LeaveImmediate(this, null);
                    t.Effect?.Invoke();
                    ChangeActive(t.Target, t.RootTarget, null);
                    ((State)t.Target).EnterImmediate(null);

                    break;
                }
            }
        }

        public bool HandleEvent(string transitionName, StateEventArg arg)
        {
            Transition t;
            if (!transitions.TryGetValue(transitionName, out t))
            {
                if (rootActive != null && rootActive.HandleEvent(transitionName, arg))
                    return true;
                return false;
            }

            if (t.RootSource != rootActive)
                return false;

            if (t.Guard != null && !t.Guard())
                return true;

            Assert.AreEqual(this, StateMachine.LCA(t.Source, t.Target));

            rootActive.LeaveImmediate(this, arg);
            t.Effect?.Invoke();
            ChangeActive(t.Target, t.RootTarget, arg);
            ((State)t.Target).EnterImmediate(arg);

            return true;
        }

        private void ChangeActive(Vertex v, Vertex rootState, StateEventArg arg)
        {
            if(v is State)
                ChangeActiveImpl(v as State, rootState as State, arg);
        }

        private void ChangeActiveImpl(State state, State rootState, StateEventArg arg)
        {
            if (active != null)
                StateMachine.OnLeaveState(active.Name);
            active = state;
            rootActive = rootState;
            StateMachine.OnEnterState(rootActive != null ? rootActive.Name : "", arg);
            //Debug.Log(string.Format("Region {0} Active: {1}", Name, rootActive != null ? rootActive.Name : "null"));
            if (rootActive is FinalState)
            {
                IsFinal = true;
                FinalEvent?.Invoke();
            }
        }

        public void Initialize()
        {
            foreach (var vertex in subvertices)
            {
                var state = vertex as State;
                if (state != null)
                    state.Initialize();
            }
        }

        public void Destroy()
        {
            foreach(var vertex in subvertices)
            {
                var state = vertex as State;
                if (state != null)
                    state.Destroy();
            }
        }
    }
}