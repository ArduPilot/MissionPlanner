using System.Collections.Generic;
using System.Timers;

namespace Transitions
{
    internal class TransitionManager
    {
        private static TransitionManager m_Instance;
        private readonly object m_Lock = new object();
        private readonly Timer m_Timer;
        private readonly IDictionary<Transition, bool> m_Transitions = new Dictionary<Transition, bool>();

        private TransitionManager()
        {
            m_Timer = new Timer(15.0);
            m_Timer.Elapsed += onTimerElapsed;
            m_Timer.Enabled = true;
        }

        public static TransitionManager getInstance()
        {
            if (m_Instance == null)
                m_Instance = new TransitionManager();
            return m_Instance;
        }

        public void register(Transition transition)
        {
            lock (m_Lock)
            {
                removeDuplicates(transition);
                m_Transitions[transition] = true;
                transition.TransitionCompletedEvent += onTransitionCompleted;
            }
        }

        private void removeDuplicates(Transition transition)
        {
            foreach (var transition1 in m_Transitions)
                removeDuplicates(transition, transition1.Key);
        }

        private void removeDuplicates(Transition newTransition, Transition oldTransition)
        {
            var transitionedProperties1 = newTransition.TransitionedProperties;
            var transitionedProperties2 = oldTransition.TransitionedProperties;
            for (var index = transitionedProperties2.Count - 1; index >= 0; --index)
            {
                var info = transitionedProperties2[index];
                foreach (var transitionedPropertyInfo in transitionedProperties1)
                    if (info.target == transitionedPropertyInfo.target &&
                        info.propertyInfo == transitionedPropertyInfo.propertyInfo)
                        oldTransition.removeProperty(info);
            }
        }

        private void onTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (m_Timer == null)
                return;
            m_Timer.Enabled = false;
            IList<Transition> transitionList;
            lock (m_Lock)
            {
                transitionList = new List<Transition>();
                foreach (var transition in m_Transitions)
                    transitionList.Add(transition.Key);
            }

            foreach (var transition in transitionList)
                transition.onTimer();
            m_Timer.Enabled = true;
        }

        private void onTransitionCompleted(object sender, Transition.Args e)
        {
            var key = (Transition) sender;
            key.TransitionCompletedEvent -= onTransitionCompleted;
            lock (m_Lock)
            {
                m_Transitions.Remove(key);
            }
        }
    }
}