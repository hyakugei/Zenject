using System;
namespace ModestTree.Zenject
{
    public class MethodProvider<T> : ProviderInternal
    {
        public delegate T Method(DiContainer c);

        private DiContainer _container;
        private Method _method;

        public MethodProvider(Method method, DiContainer container)
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
            Util.Assert(obj != null, "Method provider returned null when looking up type '" + typeof(T).Name + "'");
            return obj;
        }
    }
}
