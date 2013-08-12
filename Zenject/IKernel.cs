using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Zenject
{
    // Interface for kernel class
    // Currently there is only one (UnityKernel) but there should be another
    // eventually, once Zenject adds support for non-unity projects
    public interface IKernel
    {
        void AddTask(ITickable task);
        void RemoveTask(ITickable task);
    }
}

