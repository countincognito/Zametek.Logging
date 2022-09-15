using System;
using System.Reflection;

namespace Zametek.Utility.Logging
{
    public static class TypeExtensions
    {
        public static bool IsInterface(this Type input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            return input.GetTypeInfo().IsInterface;
        }

        public static bool IsInterface<T>(this T input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            return typeof(T).IsInterface();
        }

        public static void ThrowIfNotInterface(this Type input)
        {
            if (input is null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            if (!input.IsInterface())
            {
                throw new InvalidOperationException($"Type {input.FullName} is not an interface.");
            }
        }

        public static void ThrowIfNotInterface<T>(this T input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            typeof(T).ThrowIfNotInterface();
        }
    }
}
