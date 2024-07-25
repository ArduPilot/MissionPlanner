using System;
using System.ComponentModel;

namespace MissionPlanner.Attributes
{
    /// <summary>
    /// Used to decorate a field with a custom name.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public sealed class DisplayFieldNameAttribute : Attribute
    {
        private readonly string _translationKey;

        public DisplayFieldNameAttribute(string translationKey)
        {
            if (String.IsNullOrEmpty(translationKey))
            {
                throw new ArgumentException("\"translationKey\" is required.");
            }
            _translationKey = translationKey;
        }

        public string TryTranslate(string defaultTo)
        {
            return MissionPlanner.Utilities.L10NU.GetString(_translationKey, defaultTo: defaultTo);
        }
    }
}
