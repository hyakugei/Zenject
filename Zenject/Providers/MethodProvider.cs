using System;
namespace Zenject
{
    public class MethodProvider<T> : ProviderInternal
    {
        public delegate T Method(IContainer c);

        private IContainer _container;
        private Method _method;

        public MethodProvider(Method method, IContainer container)
        {
            _method = method;
            _container = container;
        }

        public override Type GetInstanceType()
        {
            return typeof(T);
        }

        public override object GetInstance()
        {
            var obj = _method(_container);
            ZenUtil.Assert(obj != null);
            return obj;
        }
    }
}
