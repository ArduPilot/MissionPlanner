using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AltitudeAngelWings
{
    public static class ServiceLocator
    {
        private class Resolver : IServiceLocator
        {
            public T Resolve<T>(string key = null)
            {
                return GetService<T>(key);
            }
        }

        private class TypeKey : IEquatable<TypeKey>
        {
            public TypeKey(Type type, string key)
            {
                Type = type;
                Key = string.IsNullOrEmpty(key) ? null : key;
            }

            private Type Type { get; }
            private string Key { get; }

            public bool Equals(TypeKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Type == other.Type && Key == other.Key;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == this.GetType() && Equals((TypeKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Key != null ? Key.GetHashCode() : 0);
                }
            }

            public static bool operator ==(TypeKey left, TypeKey right)
            {
                return Equals(left, right);
            }

            public static bool operator !=(TypeKey left, TypeKey right)
            {
                return !Equals(left, right);
            }
        }

        private static readonly object Lock = new object();
        private static readonly IDictionary<TypeKey, object> ServiceInstances = new Dictionary<TypeKey, object>();
        private static readonly IDictionary<TypeKey, Func<IServiceLocator, object>> ServiceRegistry = new Dictionary<TypeKey, Func<IServiceLocator, object>>();

        public static T GetService<T>(string key = null)
        {
            var typeKey = new TypeKey(typeof(T), key);
            if (ServiceInstances.ContainsKey(typeKey))
            {
                return (T)ServiceInstances[typeKey];
            }
            lock (Lock)
            {
                if (!ServiceRegistry.ContainsKey(typeKey))
                {
                    throw new InvalidOperationException($"The type {typeof(T)} is not registered.");
                }
                var resolve = ServiceRegistry[typeKey](new Resolver());
                ServiceInstances[typeKey] = resolve;
                return (T)resolve;
            }
        }

        public static void Register<T>(Func<IServiceLocator, T> constructor) => Register(null, constructor);

        public static void Register<T>(string key, Func<IServiceLocator, T> constructor)
        {
            var typeKey = new TypeKey(typeof(T), key);
            lock (Lock)
            {
                if (ServiceRegistry.ContainsKey(typeKey))
                {
                    throw new InvalidOperationException($"The type {typeof(T)} is already registered.");
                }
                ServiceRegistry.Add(typeKey, l => constructor(l));
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

        public static void ConfigureFromAssembly(Assembly assembly)
        {
            if (assembly.IsDynamic) return;
            foreach (var configuration in assembly.GetExportedTypes()
                 .Where(t => t.IsClass 
                             && !t.IsAbstract
                             && typeof(IServiceLocatorConfiguration).IsAssignableFrom(t)
                             && t.GetConstructors().Any(c => c.IsPublic
                                                             && c.GetParameters().Length == 0))
                 .Select(Activator.CreateInstance)
                 .Cast<IServiceLocatorConfiguration>())
            {
                configuration.Configure();
            }
        }
    }
}