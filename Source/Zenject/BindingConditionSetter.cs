using System;
using System.Collections.Generic;

namespace ModestTree.Zenject
{
    public class ResolveContext
    {
        public Type target;
        public string name;
        public List<Type> parents;
    }

    public delegate bool BindingCondition(ResolveContext c);

    public class BindingConditionSetter
    {
        private ProviderInternal _provider;

        public BindingConditionSetter(ProviderInternal provider)
        {
            _provider = provider;
        }

        public void When(BindingCondition condition)
        {
            _provider.SetCondition(condition);
        }

        public void WhenInjectedInto<T>()
        {
            _provider.SetCondition(r => r.target == typeof (T));
        }

        public void WhenInjectedIntoField<T>(string fieldName)
        {
            _provider.SetCondition(r => r.target == typeof (T) && r.name == fieldName);
        }
    }
}
