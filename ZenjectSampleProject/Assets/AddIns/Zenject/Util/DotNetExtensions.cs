using System;
using System.Collections.Generic;
using System.Linq;

namespace ModestTree
{
    public static class DotNetExtensions
    {
        public static bool DerivesFrom<T>(this Type a)
        {
            return DerivesFrom(a, typeof(T));
        }

        // This seems easier to think about than IsAssignableFrom
        public static bool DerivesFrom(this Type a, Type b)
        {
            return b.IsAssignableFrom(a);
        }
    }
}

