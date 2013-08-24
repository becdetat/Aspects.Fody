using System;

namespace Aspects.Fody.Extensions
{
    public static class TypeExtensions
    {
        public static T GetStaticFieldValue<T>(this Type type, string name)
        {
            return (T) type.GetField(name).GetValue(null);
        }
    }
}