using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace AltitudeAngelWings
{
    public static class ReactiveExtensions
    {
        public static IObservable<T> RepeatLastValue<T>(this IObservable<T> source, TimeSpan repeatInterval)
        {
            return source.Select(s => Observable
                    .Interval(repeatInterval)
                    .Select(_ => s)
                    .StartWith(s))
                .Switch();
        }
    }
}
