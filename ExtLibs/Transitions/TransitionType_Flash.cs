using System.Collections.Generic;

namespace Transitions
{
    public class TransitionType_Flash : TransitionType_UserDefined
    {
        public TransitionType_Flash(int iNumberOfFlashes, int iFlashTime)
        {
            var num1 = 100.0 / iNumberOfFlashes;
            var elements = (IList<TransitionElement>) new List<TransitionElement>();
            for (var index = 0; index < iNumberOfFlashes; ++index)
            {
                var num2 = index * num1;
                var endTime1 = num2 + num1;
                var endTime2 = (num2 + endTime1) / 2.0;
                elements.Add(new TransitionElement(endTime2, 100.0, InterpolationMethod.EaseInEaseOut));
                elements.Add(new TransitionElement(endTime1, 0.0, InterpolationMethod.EaseInEaseOut));
            }

            setup(elements, iFlashTime * iNumberOfFlashes);
        }
    }
}