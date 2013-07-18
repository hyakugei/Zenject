using System;
using System.Collections.Generic;

namespace Zenject
{
    public class SingletonInstanceProvider : ProviderInternal
    {
        object _instance;

        public SingletonInstanceProvider(object instance)
        {
            _instance = instance;
        }

        public override object Get()
        {
            return _instance;
        }
    }
}
