namespace Zenject
{
    public abstract class ProviderInternal : IProvider
    {
        private BindingCondition _condition = delegate { return true; };

        public BindingCondition GetCondition()
        {
            return _condition;
        }

        public void SetCondition(BindingCondition condition)
        {
            _condition = condition;
        }

        public abstract object Get();

        public virtual void OnRemoved()
        {
        }
    }
}
