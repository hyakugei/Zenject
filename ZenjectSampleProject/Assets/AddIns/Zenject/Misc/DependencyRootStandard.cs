using System.Collections.Generic;
using UnityEngine;

namespace ModestTree.Zenject
{
    public class DependencyRootStandard : IDependencyRoot
    {
        // Usually we pass dependencies via contructor injection
        // but since we define a root so often (eg. unit tests)
        // just use [Inject] for the root classes

        [Inject]
        IKernel _kernel = null;

        [Inject]
        EntryPointInitializer _initializer = null;

        public virtual void Start()
        {
            Debug.Log("Initializing dependency root");
            _initializer.Initialize();
        }
    }

    public class DependencyRootStandard<TRoot> : DependencyRootStandard
        where TRoot : class
    {
        [Inject]
        TRoot _root = null;
    }
}
