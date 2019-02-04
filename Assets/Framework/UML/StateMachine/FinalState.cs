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

        protected override void OnEnter(EnterEventArg arg)
        {
        }

        protected override void OnLeave()
        {
        }

        protected override void OnUpdate()
        {
        }
    }
}

