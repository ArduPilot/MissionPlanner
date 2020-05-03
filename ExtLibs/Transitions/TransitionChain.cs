using System.Collections.Generic;

namespace Transitions
{
    internal class TransitionChain
    {
        private readonly LinkedList<Transition> m_listTransitions = new LinkedList<Transition>();

        public TransitionChain(params Transition[] transitions)
        {
            foreach (var transition in transitions)
                m_listTransitions.AddLast(transition);
            runNextTransition();
        }

        private void runNextTransition()
        {
            if (m_listTransitions.Count == 0)
                return;
            var transition = m_listTransitions.First.Value;
            transition.TransitionCompletedEvent += onTransitionCompleted;
            transition.run();
        }

        private void onTransitionCompleted(object sender, Transition.Args e)
        {
            ((Transition) sender).TransitionCompletedEvent -= onTransitionCompleted;
            m_listTransitions.RemoveFirst();
            runNextTransition();
        }
    }
}