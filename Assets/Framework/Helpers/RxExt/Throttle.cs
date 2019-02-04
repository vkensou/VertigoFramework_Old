using System;
using UnityEngine;

namespace UniRx.Operators
{
    internal class SystemThrottleObservable<T> : OperatorObservableBase<T>
    {
        readonly IObservable<T> source;
        readonly float dueTime;

        public SystemThrottleObservable(IObservable<T> source, float delta)
            :base(false)
        {
            this.source = source;
            this.dueTime = delta;
        }

        protected override IDisposable SubscribeCore(IObserver<T> observer, IDisposable cancel)
        {
            return new SystemThrottle(this, observer, cancel).Run();
        }

        class SystemThrottle : OperatorObserverBase<T, T>
        {
            readonly SystemThrottleObservable<T> parent;
            readonly object gate = new object();
            T latestValue = default(T);
            bool hasValue = false;
            SerialDisposable cancelable;
            ulong id = 0;
            float time = 0;

            public SystemThrottle(SystemThrottleObservable<T> parent, IObserver<T> observer, IDisposable cancel) : base(observer, cancel)
            {
                this.parent = parent;
            }

            public IDisposable Run()
            {
                time = 0;
                cancelable = new SerialDisposable();
                var subscription = parent.source.Subscribe(this);

                return StableCompositeDisposable.Create(cancelable, subscription);
            }

            void OnNext(ulong currentid)
            {
                lock (gate)
                {
                    if (hasValue && id == currentid)
                    {
                        observer.OnNext(latestValue);
                    }
                    hasValue = false;
                }
            }

            public override void OnNext(T value)
            {
                float ctime = Time.time;
                float d = ctime - time;
                if(d > parent.dueTime)
                {
                    time = ctime;
                    observer.OnNext(value);
                }
            }

            public override void OnError(Exception error)
            {
                cancelable.Dispose();

                lock (gate)
                {
                    hasValue = false;
                    id = unchecked(id + 1);
                    try { observer.OnError(error); } finally { Dispose(); }
                }
            }

            public override void OnCompleted()
            {
                cancelable.Dispose();

                lock (gate)
                {
                    if (hasValue)
                    {
                        observer.OnNext(latestValue);
                    }
                    hasValue = false;
                    id = unchecked(id + 1);
                    try { observer.OnCompleted(); } finally { Dispose(); }
                }
            }
        }
    }
}