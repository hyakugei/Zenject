using System.Collections.Generic;
using UnityEngine;

namespace Zenject
{
    public class UnityKernel : MonoBehaviour
    {
        [Inject]
        public Kernel _kernel;

        public void Update()
        {
            _kernel.Update();
        }
    }
}

