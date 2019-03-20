using System;
using System.Collections.Generic;

namespace AltitudeAngelWings
{
    public static class ServiceLocator
    {
        private class Resolver : IServiceLocator
        {
            public T Resolve<T>()
            {
                return GetService<T>();
            }
        }

        private static readonly object Lock = new object();
        private static readonly IDictionary<Type, object> ServiceInstances = new Dictionary<Type, object>();
        private static readonly IDictionary<Type, Func<IServiceLocator, object>> ServiceRegistry = new Dictionary<Type, Func<IServiceLocator, object>>();

        public static T GetService<T>()
        {
            if (ServiceInstances.ContainsKey(typeof(T)))
            {
                return (T)ServiceInstances[typeof(T)];
            }
            lock (Lock)
            {
                if (!ServiceRegistry.ContainsKey(typeof(T)))
                {
                    throw new InvalidOperationException($"The type {typeof(T)} is not registered.");
                }
                var resolve = ServiceRegistry[typeof(T)](new Resolver());
                ServiceInstances[typeof(T)] = resolve;
                return (T) resolve;
            }
        }

        public static void Register<T>(Func<IServiceLocator, T> constructor)
        {
            lock (Lock)
            {
                if (ServiceRegistry.ContainsKey(typeof(T)))
                {
                    throw new InvalidOperationException($"The type {typeof(T)} is already registered.");
                }
                ServiceRegistry.Add(typeof(T), l => constructor(l));
            }
        }

        public static void Clear()
        {
            lock (Lock)
            {
                ServiceRegistry.Clear();
                foreach (var instance in ServiceInstances.Values)
                {
                    if (instance is IDisposable dispose)
                    {
                        dispose.Dispose();
                    }
                }
                ServiceInstances.Clear();
            }
        }
    }
}