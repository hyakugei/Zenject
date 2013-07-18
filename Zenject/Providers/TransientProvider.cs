namespace Zenject
{
    public class TransientProvider<T> : ProviderInternal
    {
        private FactoryBase<T> _factory;

        public TransientProvider(IContainer container)
        {
            _factory = new Factory<T>(container);
        }

        public override object Get()
        {
            var obj = _factory.Create();
            ZenUtil.Assert(obj != null);
            return obj;
        }
    }
}
