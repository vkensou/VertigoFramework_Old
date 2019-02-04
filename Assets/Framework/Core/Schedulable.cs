using UnityEngine;
using System.Collections;
using UniRx;
using System;
using UniRx.Operators;

public class Schedulable
{
    private float time = 0;
    private void StepTime(float delta)
    {
        time += delta;
    }

    private float Now => time;

    public IObservable<int> IntervalAsObservable(float delta)
    {
        return new SystemTimerObservable(delta, this);
    }

    public IDisposable Schedule(Action action)
    {
        return Schedule(0, action);
    }

    public IDisposable Schedule(float dueTime, Action action)
    {
        var item = new ScheduledItem(Now + dueTime, action);
        m_timerQueue.Enqueue(item);
        return item.Cancellation;
    }

    public IDisposable SchedulePeriodic(float dueTime, Action action)
    {
        var item = new ScheduledItem(Now + dueTime, action, dueTime);
        m_timerQueue.Enqueue(item);
        return item.Cancellation;
    }

    public void ProcessSchedule(float delta)
    {
        StepTime(delta);
        float now = Now;
        while (m_timerQueue.Count > 0)
        {
            var item = m_timerQueue.Peek();
            if (item.time < now)
            {
                m_timerQueue.Dequeue();
                if (!item.IsCanceled)
                {
                    item.action();
                    if (item.periodic)
                    {
                        item.time = now + item.dueTime;
                        m_timerQueue.Enqueue(item);
                    }
                }
            }
            else
                break;
        }
    }

    private class ScheduledItem : IComparable<ScheduledItem>
    {
        private readonly BooleanDisposable _disposable = new BooleanDisposable();

        public float time;
        public Action action;
        public bool periodic = false;
        public float dueTime = 0;

        public int CompareTo(ScheduledItem other)
        {
            return time.CompareTo(other.time);
        }

        public ScheduledItem(float time, Action action)
        {
            this.time = time;
            this.action = action;
        }

        public ScheduledItem(float time, Action action, float dueTime)
        {
            this.time = time;
            this.action = action;
            this.periodic = true;
            this.dueTime = dueTime;
        }

        public void Invoke()
        {
            if (!_disposable.IsDisposed)
            {
                action();
            }
        }

        public IDisposable Cancellation
        {
            get
            {
                return _disposable;
            }
        }

        /// <summary>
        /// Gets whether the work item has received a cancellation request.
        /// </summary>
        public bool IsCanceled
        {
            get { return _disposable.IsDisposed; }
        }
    }

    private Vertigo.PriorityQueue<ScheduledItem> m_timerQueue = new Vertigo.PriorityQueue<ScheduledItem>();

    private class SystemTimerObservable : OperatorObservableBase<int>
    {
        readonly float dueTime;
        readonly Schedulable schedulable;

        public SystemTimerObservable(float delta, Schedulable schedulable)
            : base(false)
        {
            this.dueTime = delta;
            this.schedulable = schedulable;
        }

        protected override IDisposable SubscribeCore(IObserver<int> observer, IDisposable cancel)
        {
            var timerObserver = new SystemTimer(observer, cancel);
            return schedulable.SchedulePeriodic(dueTime, timerObserver.OnNext);
        }

        class SystemTimer : OperatorObserverBase<int, int>
        {
            int index = 0;


            public SystemTimer(IObserver<int> observer, IDisposable cancel)
                : base(observer, cancel)
            {
            }

            public void OnNext()
            {
                try
                {
                    base.observer.OnNext(index++);
                }
                catch
                {
                    Dispose();
                    throw;
                }
            }

            public override void OnNext(int value)
            {
            }

            public override void OnError(Exception error)
            {
                try { observer.OnError(error); }
                finally { Dispose(); }
            }

            public override void OnCompleted()
            {
                try { observer.OnCompleted(); }
                finally { Dispose(); }
            }
        }
    }



}
