using System;
namespace ModestTree.Zenject
{
    public class TransientProvider<T> : ProviderInternal
    {
        private IFactory<T> _factory;

        public TransientProvider(DiContainer container)
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
            Util.Assert(obj != null);
            return obj;
        }
    }
}
