using System;

namespace Transitions
{
    public class TransitionType_EaseInEaseOut : ITransitionType
    {
        private readonly double m_dTransitionTime;

        public TransitionType_EaseInEaseOut(int iTransitionTime)
        {
            if (iTransitionTime <= 0)
                throw new Exception("Transition time must be greater than zero.");
            m_dTransitionTime = iTransitionTime;
        }

        public void onTimer(int iTime, out double dPercentage, out bool bCompleted)
        {
            var dElapsed = iTime / m_dTransitionTime;
            dPercentage = Utility.convertLinearToEaseInEaseOut(dElapsed);
            if (dElapsed >= 1.0)
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