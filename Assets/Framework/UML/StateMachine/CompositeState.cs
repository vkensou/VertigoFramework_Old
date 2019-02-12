using System.Collections.Generic;

namespace UML
{
    public class CompositeState : State
    {
        Dictionary<string, Region> regions = new Dictionary<string, Region>();
        Dictionary<string, ConnectionPointReference> connections;
        Dictionary<string, Pseudostate> connectionPoints;
        protected Region firstRegion;

        public CompositeState(string name)
            : base(name)
        {
        }

        public Region Region => firstRegion;

        public Region CreateRegion(string name)
        {
            var region = new Region(name, this);
            if (firstRegion == null)
                firstRegion = region;
            regions.Add(name, region);
            return region;
        }

        internal override void EnterImmediate(StateEventArg arg)
        {
            base.EnterImmediate(arg);
            EnterChildren(arg);
        }

        internal override void EnterFromParent(StateEventArg arg)
        {
            base.EnterFromParent(arg);
            EnterChildren(arg);
        }

        internal override void EnterFromChild(Region r, StateEventArg arg)
        {
            base.EnterFromChild(r, arg);
            foreach (var region in regions)
            {
                if (region.Value != r)
                    region.Value.Enter(arg);
            }
        }

        private void EnterChildren(StateEventArg arg)
        {
            foreach (var region in regions)
            {
                region.Value.Enter(arg);
            }
        }

        internal override void LeaveImmediate(Region lca, StateEventArg arg)
        {
            LeaveChildren(arg);
            base.LeaveImmediate(lca, arg);
        }

        internal override void LeaveFromParent(StateEventArg arg)
        {
            LeaveChildren(arg);
            base.LeaveFromParent(arg);
        }

        private void LeaveChildren(StateEventArg arg)
        {
            foreach (var region in regions)
            {
                region.Value.Leave(arg);
            }
        }

        internal override void Update()
        {
            base.Update();
            foreach (var region in regions)
            {
                region.Value.Update();
            }
        }

        internal override void Initialize()
        {
            base.Initialize();
            foreach (var region in regions)
            {
                region.Value.Initialize();
            }
        }

        internal override void Destroy()
        {
            foreach (var region in regions)
            {
                region.Value.Destroy();
            }
            base.Destroy();
        }

        internal override bool HandleEvent(string transitionName, StateEventArg arg)
        {
            foreach (var region in regions)
            {
                if (region.Value.HandleEvent(transitionName, arg))
                    return true;
            }
            return false;
        }
    }
}