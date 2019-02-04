using System.Collections.Generic;

namespace UML
{
    public abstract class Vertex
    {
        public string Name { get; }
        public Region Container { get; set; }
        public Vertex Next { get; set; }
        List<Transition> incomings;
        List<Transition> outgoings;

        public Vertex(string name)
        {
            Name = name;
        }

        public StateMachine StateMachine
        {
            get
            {
                if (Container != null)
                    return Container.StateMachine;
                else
                    return GetContainningStateMachine();
            }
        }

        public bool IsContainedInState(State s)
        {
            if (s.Kind == StateKind.Composite || Container == null)
                return false;
            else
            {
                if (Container.State == s)
                    return true;
                else
                    return Container.State.IsContainedInState(s);
            }
        }

        public bool IsContainedInRegion(Region r)
        {
            if (Container == r)
                return true;
            else
            {
                if (r.State == null)
                    return false;
                else
                    return Container.State.IsContainedInRegion(r);
            }
        }

        protected abstract StateMachine GetContainningStateMachine();
    }
}