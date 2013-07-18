using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Zenject
{
    public class EntryPointInitializer : ITickable
    {
        public int TickPriority
        {
            // We need to be the first tickable that's run, since
            // initialization should always occur before any update call
            // so use the minimum priority to ensure this is the case
            get {  return int.MinValue; }
        }

        public const int UnityStartInitPriority = 10000;

        [Inject]
        public List<IEntryPoint> _entryPoints;

        Kernel _kernel;

        List<IEntryPoint> _entryPointsEarly = new List<IEntryPoint>();
        List<IEntryPoint> _entryPointsLate = new List<IEntryPoint>();

        public EntryPointInitializer(List<IEntryPoint> entryPoints, Kernel kernel)
        {
            _kernel = kernel;
            _entryPoints = entryPoints;

            InitEntryPoints();
            TriggerEntryPoints();

            _kernel.AddTask(this);
        }

        void TriggerEntryPoints()
        {
            _entryPointsEarly.ForEach(e => e.Initialize());
        }

        void InitEntryPoints()
        {
            _entryPoints.Sort(delegate (IEntryPoint e1, IEntryPoint e2)
                    {
                    return (e1.InitPriority.CompareTo(e2.InitPriority));
                    });

            foreach (var entryPoint in _entryPoints)
            {
                if (entryPoint.InitPriority < UnityStartInitPriority)
                {
                    _entryPointsEarly.Add(entryPoint);
                } 
                else
                {
                    _entryPointsLate.Add(entryPoint);
                }
            }
        }

        // Do late initialize in Update() to ensure that all monobehavior Start() methods have 
        // been called
        public void Update()
        {
            _entryPointsLate.ForEach(e => e.Initialize());
            _kernel.RemoveTask(this);
        }
    }
}
