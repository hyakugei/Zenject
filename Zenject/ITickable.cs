using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Zenject
{
    public interface ITickable
    {
        int TickPriority { get; }

        void Update();
    }
}
