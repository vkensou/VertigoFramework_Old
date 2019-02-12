namespace UML
{
    public sealed class FinalState : State
    {
        public FinalState()
            :base("Final")
        {

        }

        public FinalState(string name)
            :base(name)
        {

        }

        protected override void OnEnter(StateEventArg arg)
        {
        }

        protected override void OnLeave(StateEventArg arg)
        {
        }

        protected override void OnUpdate()
        {
        }
    }
}

