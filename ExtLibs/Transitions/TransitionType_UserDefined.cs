using System;
using System.Collections.Generic;

namespace Transitions
{
    public class TransitionType_UserDefined : ITransitionType
    {
        private double m_dTransitionTime;
        private IList<TransitionElement> m_Elements;
        private int m_iCurrentElement;

        public TransitionType_UserDefined()
        {
        }

        public TransitionType_UserDefined(IList<TransitionElement> elements, int iTransitionTime)
        {
            setup(elements, iTransitionTime);
        }

        public void onTimer(int iTime, out double dPercentage, out bool bCompleted)
        {
            var dTimeFraction = iTime / m_dTransitionTime;
            double dStartTime;
            double dEndTime;
            double dStartValue;
            double dEndValue;
            InterpolationMethod eInterpolationMethod;
            getElementInfo(dTimeFraction, out dStartTime, out dEndTime, out dStartValue, out dEndValue,
                out eInterpolationMethod);
            var num = dEndTime - dStartTime;
            var dElapsed = (dTimeFraction - dStartTime) / num;
            double dPercentage1;
            switch (eInterpolationMethod)
            {
                case InterpolationMethod.Linear:
                    dPercentage1 = dElapsed;
                    break;
                case InterpolationMethod.Accleration:
                    dPercentage1 = Utility.convertLinearToAcceleration(dElapsed);
                    break;
                case InterpolationMethod.Deceleration:
                    dPercentage1 = Utility.convertLinearToDeceleration(dElapsed);
                    break;
                case InterpolationMethod.EaseInEaseOut:
                    dPercentage1 = Utility.convertLinearToEaseInEaseOut(dElapsed);
                    break;
                default:
                    throw new Exception("Interpolation method not handled: " + eInterpolationMethod);
            }

            dPercentage = Utility.interpolate(dStartValue, dEndValue, dPercentage1);
            if (iTime >= m_dTransitionTime)
            {
                bCompleted = true;
                dPercentage = dEndValue;
            }
            else
            {
                bCompleted = false;
            }
        }

        public void setup(IList<TransitionElement> elements, int iTransitionTime)
        {
            m_Elements = elements;
            m_dTransitionTime = iTransitionTime;
            if (elements.Count == 0)
                throw new Exception(
                    "The list of elements passed to the constructor of TransitionType_UserDefined had zero elements. It must have at least one element.");
        }

        private void getElementInfo(double dTimeFraction, out double dStartTime, out double dEndTime,
            out double dStartValue, out double dEndValue, out InterpolationMethod eInterpolationMethod)
        {
            int count;
            for (count = m_Elements.Count; m_iCurrentElement < count; ++m_iCurrentElement)
            {
                var num = m_Elements[m_iCurrentElement].EndTime / 100.0;
                if (dTimeFraction < num)
                    break;
            }

            if (m_iCurrentElement == count)
                m_iCurrentElement = count - 1;
            dStartTime = 0.0;
            dStartValue = 0.0;
            if (m_iCurrentElement > 0)
            {
                var element = m_Elements[m_iCurrentElement - 1];
                dStartTime = element.EndTime / 100.0;
                dStartValue = element.EndValue / 100.0;
            }

            var element1 = m_Elements[m_iCurrentElement];
            dEndTime = element1.EndTime / 100.0;
            dEndValue = element1.EndValue / 100.0;
            eInterpolationMethod = element1.InterpolationMethod;
        }
    }
}