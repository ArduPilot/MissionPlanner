using System;
using System.Collections.Generic;
using System.Reflection;

namespace ProjNet
{
    internal static class TypeExtensions
    {
#if !HAS_SYSTEM_TYPE_GETCONSTRUCTORS
#if !HAS_SYSTEM_REFLECTION_TYPEINFO
#error Must have either one or the other.
#endif
        internal static IEnumerable<ConstructorInfo> GetConstructors(this Type type) => type.GetTypeInfo().DeclaredConstructors;
#endif

#if !HAS_SYSTEM_TYPE_ISASSIGNABLEFROM
#if !HAS_SYSTEM_REFLECTION_TYPEINFO
#error Must have either one or the other.
#endif
        internal static bool IsAssignableFrom(this Type type, Type other) => type.GetTypeInfo().IsAssignableFrom(other.GetTypeInfo());
#endif
    }
}
