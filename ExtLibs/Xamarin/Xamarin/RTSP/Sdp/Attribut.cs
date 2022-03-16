using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Rtsp.Sdp
{
    public class Attribut
    {
        private static readonly Dictionary<string, Type> attributMap = new Dictionary<string, Type>()
        {
            {AttributRtpMap.NAME,typeof(AttributRtpMap)},
            {AttributFmtp.NAME,typeof(AttributFmtp)},
        };


        public virtual string Key { get; private set; }
        public virtual string Value { get; protected set; }

        public static void RegisterNewAttributeType(string key, Type attributType)
        {
            if(!attributType.IsSubclassOf(typeof(Attribut)))
                throw new ArgumentException("Type must be subclass of Rtsp.Sdp.Attribut","attributType");

            attributMap[key] = attributType;
        }

        

        public Attribut()
        {
        }

        public Attribut(string key)
        {
            Key = key;
        }


        public static Attribut ParseInvariant(string value)
        {
            if(value == null)
                throw new ArgumentNullException("value");

            Contract.EndContractBlock();

            var listValues = value.Split(new char[] {':'}, 2);
            

            Attribut returnValue;

            // Call parser of child type
            Type childType;
            attributMap.TryGetValue(listValues[0], out childType);
            if (childType != null)
            {
                var defaultContructor = childType.GetConstructor(Type.EmptyTypes);
                returnValue = defaultContructor.Invoke(Type.EmptyTypes) as Attribut;
            }
            else
            {
                returnValue = new Attribut(listValues[0]);
            }
            // Parse the value. Note most attributes have a value but recvonly does not have a value
            if (listValues.Count() > 1) returnValue.ParseValue(listValues[1]);

            return returnValue;
        }

        protected virtual void ParseValue(string value)
        {
            Value = value;
        }

        
    }
}
