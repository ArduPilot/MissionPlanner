using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AltitudeAngelWings
{
    public static class ReactiveExtensions
    {
        public static IObservable<T> RepeatLastValue<T>(this IObservable<T> source, TimeSpan repeatInterval)
            => source.Select(s => Observable
                    .Interval(repeatInterval)
                    .Select(_ => s)
                    .StartWith(s))
                .Switch();

        public static IDisposable SubscribeWithAsync<T>(this IObservable<T> source, Func<T, CancellationToken, Task> asyncFunc)
            => source
                .Select(i => Observable.FromAsync(async ct =>
                {
                    try
                    {
                        await asyncFunc(i, ct);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }))
                .Switch()
                .Subscribe(i => {});
    }
}
