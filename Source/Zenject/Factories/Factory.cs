using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;

namespace ModestTree.Zenject
{
    public class Factory<TContract, TConcrete> : FactoryBase<TContract> where TConcrete : TContract
    {
        public Factory(DiContainer container)
            : base(container)
        {
        }

        public override TContract Create(params object[] constructorArgs)
        {
            return (TContract)Create(typeof(TConcrete), constructorArgs);
        }
    }

    public class Factory<TContract> : FactoryBase<TContract>
    {
        public Factory(DiContainer container)
            : base(container)
        {
        }

        public override TContract Create(params object[] constructorArgs)
        {
            return (TContract)Create(typeof(TContract), constructorArgs);
        }
    }
}
