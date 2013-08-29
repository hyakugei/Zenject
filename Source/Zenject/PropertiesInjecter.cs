using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModestTree.Zenject
{
    // Iterate over fields/properties on a given object and inject any with the [Inject] attribute
    class PropertiesInjecter
    {
        private DiContainer _container;

        public PropertiesInjecter(DiContainer container)
        {
            _container = container;
        }

        public void Inject(object injectable)
        {
            Util.Assert(injectable != null);

            var members = ZenUtil.GetMemberDependencies(injectable.GetType());

            var parentDependencies = new List<Type>(_container.LookupsInProgress);

            foreach (var member in members)
            {
                var context = new ResolveContext() 
                { 
                    target = injectable.GetType(), 
                    name = member.Name,
                    parents = parentDependencies,
                };

                var injectAttr = ZenUtil.GetInjectAttribute(member);
                Util.Assert(injectAttr != null);

                if (member is FieldInfo)
                {
                    var info = member as FieldInfo;
                    var valueObj = ResolveField(info.FieldType, context);

                    Util.Assert(valueObj != null || injectAttr.optional,
                                "Unable to find field with type '" + info.FieldType +
                                "' when injecting dependencies into '" + injectable + "'");

                    info.SetValue(injectable, valueObj);
                }
                else if (member is PropertyInfo)
                {
                    var info = member as PropertyInfo;
                    var valueObj = ResolveField(info.PropertyType, context);

                    Util.Assert(valueObj != null || injectAttr.optional,
                            "Unable to find property with type '" + info.PropertyType +
                            "' when injecting dependencies into '" + injectable + "'");

                    info.SetValue(injectable, valueObj, null);
                }
                else
                {
                    Util.Assert(false);
                }
            }
        }

        object ResolveField(Type desiredType, ResolveContext context)
        {
            var valueObj = _container.Resolve(desiredType, context);

            if (valueObj == null)
            {
                // If it's a list it might map to a collection
                if (desiredType.FullName.StartsWith("System.Collections.Generic.List"))
                {
                    var subTypes = desiredType.GetGenericArguments();

                    if (subTypes.Length == 1)
                    {
                        var subType = subTypes[0];
                        valueObj = _container.ResolveMany(subType, context);
                    }
                }
            }

            return valueObj;
        }
    }
}
