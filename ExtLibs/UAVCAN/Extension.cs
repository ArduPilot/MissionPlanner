using System;
using System.Collections.Generic;
using System.Text;
using float32 = System.Single;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UAVCAN
{
    public static class Extension
    {
        public static T ByteArrayToUAVCANMsg<T>(this byte[] transfer, int startoffset) where T : new()
        {
            var ans = ((IUAVCANSerialize) new T());
            ans.decode(new uavcan.CanardRxTransfer(transfer.Skip(startoffset).ToArray()));
            return (T) ans;
        }

        public static IEnumerable<Tuple<T, T>> NowNextBy2<T>(this IEnumerable<T> list)
        {
            T now = default(T);
            T next = default(T);

            int a = -1;
            foreach (var item in list)
            {
                a++;
                now = next;
                next = item;
                if (a % 2 == 0)
                    continue;
                yield return new Tuple<T, T>(now, next);
            }
        }
    }
}