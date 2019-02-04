using System;

namespace UML
{
    public enum TransitionKind
    {
        Internal,
        Local,
        External
    }

    public class Transition
    {
        public TransitionKind Kind { get; } = TransitionKind.External;
        public Vertex Source { get; }
        public Vertex RootSource { get; }
        public Vertex Target { get; }
        public Vertex RootTarget { get; }
        public Action Effect { get; }
        public Func<bool> Guard { get; }
        int triggers;

        public Transition(Vertex source, Vertex rootSource, Vertex target, Vertex rootTarget, Func<bool> guard, Action effect, TransitionKind kind)
        {
            Source = source;
            RootSource = rootSource;
            Target = target;
            RootTarget = rootTarget;
            Guard = guard;
            Effect = effect;
            Kind = kind;
        }
    }
}

