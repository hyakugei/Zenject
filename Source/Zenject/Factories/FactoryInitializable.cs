using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Permissions;

namespace ModestTree.Zenject
{
    // These factories can be used in cases where the objects we're creating needs to do some
    // extra work after creation
    // It's often undesirable to put too much logic in the constructor 
    // (http://blog.vuscode.com/malovicn/archive/2009/10/16/inversion-of-control-single-responsibility-principle-and-nikola-s-laws-of-dependency-injection.aspx)
    // (http://blog.ploeh.dk/2011/03/03/InjectionConstructorsshouldbesimple/)
    // so this is usually a good alternative
    public class FactoryInitializable<TContract, TConcrete> : FactoryBase<TContract> 
        where TConcrete : TContract
        where TContract : IInitializable
    {
        public FactoryInitializable(DiContainer container)
            : base(container)
        {
        }

        public override TContract Create(params object[] constructorArgs)
        {
            var obj = (TContract)Create(typeof(TConcrete), constructorArgs);
            obj.Initialize();
            return obj;
        }
    }

    public class FactoryInitializable<TContract> : FactoryBase<TContract> where TContract : IInitializable
    {
        public FactoryInitializable(DiContainer container)
            : base(container)
        {
        }

        public override TContract Create(params object[] constructorArgs)
        {
            var obj = (TContract)Create(typeof(TContract), constructorArgs);
            obj.Initialize();
            return obj;
        }
    }
}
