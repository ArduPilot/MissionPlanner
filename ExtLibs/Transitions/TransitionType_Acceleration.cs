using System;

namespace Transitions
{
    public class TransitionType_Acceleration : ITransitionType
    {
        private readonly double m_dTransitionTime;

        public TransitionType_Acceleration(int iTransitionTime)
        {
            if (iTransitionTime <= 0)
                throw new Exception("Transition time must be greater than zero.");
            m_dTransitionTime = iTransitionTime;
        }

        public void onTimer(int iTime, out double dPercentage, out bool bCompleted)
        {
            var num = iTime / m_dTransitionTime;
            dPercentage = num * num;
            if (num >= 1.0)
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