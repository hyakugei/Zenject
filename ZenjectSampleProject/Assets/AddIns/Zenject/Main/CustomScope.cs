using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModestTree.Zenject
{
    // This class should ONLY be used the following way:
    //
    //  using (var scope = CreateScope())
    //  {
    //      scope.Bind(playerWrapper);
    //      ... 
    //  }
    public class CustomScope : IDisposable
    {
        DiContainer _container;
        List<Type> _scopeContracts = new List<Type>();

        public CustomScope(DiContainer container)
        {
            _container = container;
        }

        public Binder<TContract> Bind<TContract>()
        {
            _scopeContracts.Add(typeof (TContract));
            return _container.Bind<TContract>();
        }

        public void Dispose()
        {
            foreach (var type in _scopeContracts)
            {
                _container.Release(type);
            }
        }
    }
}
