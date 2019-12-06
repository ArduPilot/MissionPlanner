using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace Transitions
{
    public class Transition
    {
        private static readonly IDictionary<Type, IManagedType>
            m_mapManagedTypes = new Dictionary<Type, IManagedType>();

        private readonly object m_Lock = new object();
        private readonly Stopwatch m_Stopwatch = new Stopwatch();
        private readonly ITransitionType m_TransitionMethod;

        static Transition()
        {
            registerType(new ManagedType_Int());
            registerType(new ManagedType_Float());
            registerType(new ManagedType_Double());
            registerType(new ManagedType_Color());
            registerType(new ManagedType_String());
        }

        public Transition(ITransitionType transitionMethod)
        {
            m_TransitionMethod = transitionMethod;
        }

        internal IList<TransitionedPropertyInfo> TransitionedProperties { get; } =
            (IList<TransitionedPropertyInfo>) new List<TransitionedPropertyInfo>();

        public event EventHandler<Args> TransitionCompletedEvent;

        public static void run(object target, string strPropertyName, object destinationValue,
            ITransitionType transitionMethod)
        {
            var transition = new Transition(transitionMethod);
            transition.add(target, strPropertyName, destinationValue);
            transition.run();
        }

        public static void run(object target, string strPropertyName, object initialValue, object destinationValue,
            ITransitionType transitionMethod)
        {
            Utility.setValue(target, strPropertyName, initialValue);
            run(target, strPropertyName, destinationValue, transitionMethod);
        }

        public static void runChain(params Transition[] transitions)
        {
            var transitionChain = new TransitionChain(transitions);
        }

        public void add(object target, string strPropertyName, object destinationValue)
        {
            var property = target.GetType().GetProperty(strPropertyName);
            if (property == null)
                throw new Exception("Object: " + target + " does not have the property: " + strPropertyName);
            var propertyType = property.PropertyType;
            if (!m_mapManagedTypes.ContainsKey(propertyType))
                throw new Exception("Transition does not handle properties of type: " + propertyType);
            if (!property.CanRead || !property.CanWrite)
                throw new Exception("Property is not both getable and setable: " + strPropertyName);
            var mapManagedType = m_mapManagedTypes[propertyType];
            var transitionedPropertyInfo = new TransitionedPropertyInfo();
            transitionedPropertyInfo.endValue = destinationValue;
            transitionedPropertyInfo.target = target;
            transitionedPropertyInfo.propertyInfo = property;
            transitionedPropertyInfo.managedType = mapManagedType;
            lock (m_Lock)
            {
                TransitionedProperties.Add(transitionedPropertyInfo);
            }
        }

        public void run()
        {
            foreach (var transitionedProperty in TransitionedProperties)
            {
                var o = transitionedProperty.propertyInfo.GetValue(transitionedProperty.target, null);
                transitionedProperty.startValue = transitionedProperty.managedType.copy(o);
            }

            m_Stopwatch.Reset();
            m_Stopwatch.Start();
            TransitionManager.getInstance().register(this);
        }

        internal void removeProperty(TransitionedPropertyInfo info)
        {
            lock (m_Lock)
            {
                TransitionedProperties.Remove(info);
            }
        }

        internal void onTimer()
        {
            double dPercentage;
            bool bCompleted;
            m_TransitionMethod.onTimer((int) m_Stopwatch.ElapsedMilliseconds, out dPercentage, out bCompleted);
            var transitionedPropertyInfoList = (IList<TransitionedPropertyInfo>) new List<TransitionedPropertyInfo>();
            lock (m_Lock)
            {
                foreach (var transitionedProperty in TransitionedProperties)
                    transitionedPropertyInfoList.Add(transitionedProperty.copy());
            }

            foreach (var transitionedPropertyInfo in transitionedPropertyInfoList)
            {
                var intermediateValue =
                    transitionedPropertyInfo.managedType.getIntermediateValue(transitionedPropertyInfo.startValue,
                        transitionedPropertyInfo.endValue, dPercentage);
                setProperty(this,
                    new PropertyUpdateArgs(transitionedPropertyInfo.target, transitionedPropertyInfo.propertyInfo,
                        intermediateValue));
            }

            if (!bCompleted)
                return;
            m_Stopwatch.Stop();
            Utility.raiseEvent(TransitionCompletedEvent, this, new Args());
        }

        private void setProperty(object sender, PropertyUpdateArgs args)
        {
            try
            {
                if (isDisposed(args.target))
                    return;
                var target = args.target as ISynchronizeInvoke;
                if (target != null && target.InvokeRequired)
                    target.BeginInvoke(new EventHandler<PropertyUpdateArgs>(setProperty), new object[2]
                    {
                        sender,
                        args
                    }).AsyncWaitHandle.WaitOne(50);
                else
                    args.propertyInfo.SetValue(args.target, args.value, null);
            }
            catch (Exception ex)
            {
            }
        }

        private bool isDisposed(object target)
        {
            if (target == null)
                return true;
            var type = target.GetType();
            var field = type.GetProperty("IsDisposed");
            if (field == null)
                return true;
            var value = (bool) field.GetValue(target, null);
            var field2 = type.GetProperty("Disposing");
            if (field2 == null)
                return true;
            var value2 = (bool) field2.GetValue(target, null);
            return value || value2;
        }

        private static void registerType(IManagedType transitionType)
        {
            var managedType = transitionType.getManagedType();
            m_mapManagedTypes[managedType] = transitionType;
        }

        public class Args : EventArgs
        {
        }

        internal class TransitionedPropertyInfo
        {
            public object endValue;
            public IManagedType managedType;
            public PropertyInfo propertyInfo;
            public object startValue;
            public object target;

            public TransitionedPropertyInfo copy()
            {
                return new TransitionedPropertyInfo
                {
                    startValue = startValue,
                    endValue = endValue,
                    target = target,
                    propertyInfo = propertyInfo,
                    managedType = managedType
                };
            }
        }

        private class PropertyUpdateArgs : EventArgs
        {
            public readonly PropertyInfo propertyInfo;
            public readonly object target;
            public readonly object value;

            public PropertyUpdateArgs(object t, PropertyInfo pi, object v)
            {
                target = t;
                propertyInfo = pi;
                value = v;
            }
        }
    }
}