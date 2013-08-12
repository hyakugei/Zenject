using System.Collections.Generic;

namespace Zenject
{
    public class DependencyRootStandard : IDependencyRoot
    {
        [Inject]
        public IKernel kernel;

        [Inject]
        public EntryPointInitializer appInitializer;
    }
}
