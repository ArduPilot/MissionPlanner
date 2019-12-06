using System;

namespace Transitions
{
    public class TransitionType_Linear : ITransitionType
    {
        private readonly double m_dTransitionTime;

        public TransitionType_Linear(int iTransitionTime)
        {
            if (iTransitionTime <= 0)
                throw new Exception("Transition time must be greater than zero.");
            m_dTransitionTime = iTransitionTime;
        }

        public void onTimer(int iTime, out double dPercentage, out bool bCompleted)
        {
            dPercentage = iTime / m_dTransitionTime;
            if (dPercentage >= 1.0)
            {
                dPercentage = 1.0;
                bCompleted = true;
            }
            else
            {
                bCompleted = false;
            }
        }
    }
}