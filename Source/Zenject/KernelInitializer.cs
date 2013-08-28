using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ModestTree.Zenject
{
    public class KernelInitializer : IEntryPoint
    {
        public int InitPriority
        {
            get
            {
                // Add tasks as early as possible
                return int.MinValue;
            }
        }

        List<ITickable> _queuedTasks;
        IKernel _kernel;

        public KernelInitializer(IKernel kernel, List<ITickable> tasks)
        {
            _queuedTasks = tasks;
            _kernel = kernel;
        }

        public void Initialize()
        {
            foreach (var task in _queuedTasks)
            {
                _kernel.AddTask(task);
            }
        }
    }
}

