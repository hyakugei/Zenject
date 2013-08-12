using System;
using System.Collections.Generic;

namespace Zenject
{
    public interface IContainer
    {
        Binder<TContract> Bind<TContract>();

        List<TContract> ResolveMany<TContract>();
        List<TContract> ResolveMany<TContract>(ResolveContext context);

        // Returns generic List<contract>
        object ResolveMany(Type contract);
        object ResolveMany(Type contract, ResolveContext context);

        TContract Resolve<TContract>();
        TContract Resolve<TContract>(ResolveContext context);

        object Resolve(Type contract);
        object Resolve(Type contract, ResolveContext context);

        void Release<TContract>();
        void Release(Type contract);

        List<Type> GetDependencyContracts<TContract>();
        List<Type> GetDependencyContracts(Type contract);

        Dictionary<Type, List<Type>> CalculateObjectGraph<TRoot>();
        Dictionary<Type, List<Type>> CalculateObjectGraph(Type contract);
    }
}
