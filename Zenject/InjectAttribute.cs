using System;

namespace Zenject
{
    [Flags]
    public enum InjectFlag
    {
        Optional = 1,
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class InjectAttribute : Attribute
    {
        public bool optional = false;

        public InjectAttribute()
        {
        }

        public InjectAttribute(InjectFlag flags)
        {
            optional = (flags | InjectFlag.Optional) > 0;
        }
    }
}
