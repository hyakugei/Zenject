using UnityEngine;

namespace Zenject
{
    public abstract class IInstaller : MonoBehaviour
    {
        protected IContainer _container;

        public void SetContainer(IContainer container)
        {
            _container = container;
        }

        public abstract void RegisterBindings();

        public virtual void Start()
        {
            // Placed here otherwise we don't get an enable/disable flag in inspector
        }
    }
}
