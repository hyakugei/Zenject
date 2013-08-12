using System;
namespace Zenject
{
    public class TransientProvider<T> : ProviderInternal
    {
        private FactoryBase<T> _factory;

        public TransientProvider(IContainer container)
        {
            _factory = new Factory<T>(container);
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override object GetInstance()
        {
            var obj = _factory.Create();
            ZenUtil.Assert(obj != null);
            return obj;
        }
    }
}
