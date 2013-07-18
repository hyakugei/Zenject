using System.Collections.Generic;

namespace Zenject
{
    public class DependencyRootStandard : IDependencyRoot
    {
        [Inject]
        public UnityKernel kernel;

        [Inject]
        public EntryPointInitializer appInitializer;
    }
}
