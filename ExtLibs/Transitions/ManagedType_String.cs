using System;

namespace Transitions
{
    internal class ManagedType_String : IManagedType
    {
        public Type getManagedType()
        {
            return typeof(string);
        }

        public object copy(object o)
        {
            return new string(((string) o).ToCharArray());
        }

        public object getIntermediateValue(object start, object end, double dPercentage)
        {
            var str1 = (string) start;
            var str2 = (string) end;
            var length1 = str1.Length;
            var length2 = str2.Length;
            var length3 = Utility.interpolate(length1, length2, dPercentage);
            var chArray = new char[length3];
            for (var index = 0; index < length3; ++index)
            {
                var ch1 = 'a';
                if (index < length1)
                    ch1 = str1[index];
                var ch2 = 'a';
                if (index < length2)
                    ch2 = str2[index];
                var ch3 = ch2 != ' '
                    ? Convert.ToChar(Utility.interpolate(Convert.ToInt32(ch1), Convert.ToInt32(ch2), dPercentage))
                    : ' ';
                chArray[index] = ch3;
            }

            return new string(chArray);
        }
    }
}