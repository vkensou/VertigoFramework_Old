using System;
using System.Collections.Generic;
using UniRx.Operators;

namespace UniRx
{
    public static class ObservableExt
    {
        public static IObservable<TSource> SystemThrottle<TSource>(this IObservable<TSource> source, float delta)
        {
            return new SystemThrottleObservable<TSource>(source, delta);
        }

    }
}