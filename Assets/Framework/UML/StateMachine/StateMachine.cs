using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace UML
{
    public class StateMachine
    {
        Dictionary<string, Region> regions = new Dictionary<string, Region>();
        Region firstRegion;
        Dictionary<string, Pseudostate> connectionPoints;
        State submachineState;
        public event Action FinalEvent;
        public event Action TerminateEvent;
        public event Action<string, StateEventArg> EnterStateEvent;
        public event Action<string> LeaveStateEvent;
        public bool IsFinal { get; private set; } = false;
        public bool IsTerminate { get; private set; }
        public bool IsStarted { get; private set; } = false;

        public Region Region => firstRegion;
        public Region CreateRegion(string name)
        {
            var region = new Region(name, this);
            if (firstRegion == null)
                firstRegion = region;
            region.FinalEvent += OnFinal;
            region.TerminateEvent += OnTerminate;
            regions.Add(name, region);
            return region;
        }

        public StateMachine AddTransition(string name, State source, State target, Func<bool> guard = null, Action effect = null, TransitionKind kind = TransitionKind.External)
        {
            Assert.AreNotEqual(name, "__Start");
            Assert.IsTrue(!(source is FinalState));

            Region lca = LCA(source, target);
            Assert.AreEqual(lca.StateMachine, this);
            Vertex rootSource = FindRoot(source, lca);
            Vertex rootTarget = FindRoot(target, lca);

            Assert.IsNotNull(rootSource);
            Assert.IsNotNull(rootTarget);

            Transition t = new Transition(source, rootSource, target, rootTarget, guard, effect, kind);
            lca.AddTransitionImpl(name, t);
            return this;
        }

        public StateMachine AddAutoTransition(State source, State target, Func<bool> guard = null, Action effect = null, TransitionKind kind = TransitionKind.External)
        {
            Assert.IsTrue(!(source is FinalState));

            Region lca = LCA(source, target);
            Assert.AreEqual(lca.StateMachine, this);
            Vertex rootSource = FindRoot(source, lca);
            Vertex rootTarget = FindRoot(target, lca);

            Assert.IsNotNull(rootSource);
            Assert.IsNotNull(rootTarget);

            Transition t = new Transition(source, rootSource, target, rootTarget, guard, effect, kind);
            lca.AddAutoTransitionImpl(t);
            return this;
        }

        public void Start()
        {
            Assert.AreEqual(false, IsStarted);
            Assert.AreNotEqual(regions.Count, 0);

            IsStarted = true;
            foreach(var region in regions)
            {
                region.Value.Enter(null);
            }
        }

        public void FireEvent(string transitionName, StateEventArg arg = null)
        {
            if (transitionName == "__Start")
                Start();
            else
                foreach (var region in regions)
                {
                    if (region.Value.HandleEvent(transitionName, arg))
                        break;
                }
        }

        public void Update()
        {
            foreach (var region in regions)
                region.Value.Update();
        }

        public void Initialize()
        {
            foreach (var region in regions)
                region.Value.Initialize();
        }

        public void Destroy()
        {
            foreach (var region in regions)
                region.Value.Leave(null);
            foreach (var region in regions)
                region.Value.Destroy();
        }

        protected void OnFinal()
        {
            if (!IsFinal)
            {
                foreach (var region in regions)
                {
                    IsFinal |= region.Value.IsFinal;
                }

                if(IsFinal)
                    FinalEvent?.Invoke();
            }
        }

        protected void OnTerminate()
        {
            TerminateEvent?.Invoke();
        }

        internal void OnEnterState(string state, StateEventArg arg)
        {
            EnterStateEvent?.Invoke(state, arg);
        }

        internal void OnLeaveState(string state)
        {
            LeaveStateEvent?.Invoke(state);
        }

        #region helper
        public static int VertexDepth(Vertex v)
        {
            int depth = 1;
            Vertex p = v.Container.State;
            while (p != null)
            {
                ++depth;
                p = p.Container.State;
            }
            return depth;
        }

        //寻找s1，s2共同祖先区域
        public static Region LCA(Vertex s1, Vertex s2)
        {
            if (s1 == s2 || s1.Container == s2.Container)
                return s1.Container;

            int depth1 = VertexDepth(s1);
            int depth2 = VertexDepth(s2);
            Vertex v1 = s1, v2 = s2;
            int vd1 = depth1, vd2 = depth2;
            if (depth1 < depth2)
            {
                v1 = s2;
                vd1 = depth2;
                v2 = s1;
                vd2 = depth1;
            }
            while(vd1 > vd2)
            {
                --vd1;
                v1 = v1.Container.State;
            }

            while(v1 != v2 && v1.Container != v2.Container)
            {
                v1 = v1.Container.State;
                v2 = v2.Container.State;
            }
            return v1.Container;
        }

        //寻找s1，s2共同祖先State
        public static State LCAState(Vertex v1, Vertex v2)
        {
            return LCA(v1, v2).State;
        }

        //s2是否是s1的祖先
        public static bool IsAncestor(Vertex s1, Vertex s2)
        {
            if (s2 == s1)
                return true;
            else if (s1.Container.StateMachine != null)
                return false;
            else
            {
                Vertex p = s1.Container.State;
                while(p != null)
                {
                    if (p == s2)
                        return true;
                    p = p.Container.State;
                }
                return false;
            }
        }

        public static Vertex FindRoot(Vertex v, Region r)
        {
            while(v.Container != r)
            {
                v = v.Container.State;
            }
            return v;
        }
        #endregion
    }
}