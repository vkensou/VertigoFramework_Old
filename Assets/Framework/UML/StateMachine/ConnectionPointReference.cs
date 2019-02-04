using System.Collections.Generic;

namespace UML
{
    public sealed class ConnectionPointReference : Vertex
    {
        State state;
        List<Pseudostate> entries;
        List<Pseudostate> exits;

        public ConnectionPointReference(string name)
            :base(name)
        {
        }

        protected override StateMachine GetContainningStateMachine()
        {
            return state.StateMachine;
        }
    }
}

