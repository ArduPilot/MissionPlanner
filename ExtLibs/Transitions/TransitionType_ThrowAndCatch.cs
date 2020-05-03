using System.Collections.Generic;

namespace Transitions
{
    public class TransitionType_ThrowAndCatch : TransitionType_UserDefined
    {
        public TransitionType_ThrowAndCatch(int iTransitionTime)
        {
            var elements = new List<TransitionElement>();
            elements.Add(new TransitionElement(50.0, 100.0, InterpolationMethod.Deceleration));
            elements.Add(new TransitionElement(100.0, 0.0, InterpolationMethod.Accleration));
            setup(elements, iTransitionTime);
        }
    }
}