using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Zenject
{
    // Iterate over fields/properties on a given object and inject any with the [Inject] attribute
    class PropertiesInjecter
    {
        private IContainer _container;

        public PropertiesInjecter(IContainer container)
        {
            _container = container;
        }

        public void Inject(object injectable)
        {
            ZenUtil.Assert(injectable != null);

            var contract = injectable.GetType();

            var members = contract.FindMembers(MemberTypes.Property | MemberTypes.Field,
                                               BindingFlags.FlattenHierarchy | BindingFlags.Public |
                                               BindingFlags.NonPublic | BindingFlags.SetProperty |
                                               BindingFlags.Instance,
                                               null, null);

            foreach (var member in members)
            {
                var attrs = member.GetCustomAttributes(typeof(InjectAttribute), true);

                InjectAttribute injectAttr = null;

                foreach (var attr in attrs)
                {
                    if (attr.GetType() == typeof(InjectAttribute))
                    {
                        injectAttr = attr as InjectAttribute;
                        break;
                    }
                }

                if (injectAttr != null)
                {
                    var context = new ResolveContext() { target = injectable.GetType(), name = member.Name };

                    if (member is FieldInfo)
                    {
                        var info = member as FieldInfo;
                        var valueObj = ResolveField(info.FieldType, context);

                        ZenUtil.Assert(valueObj != null || injectAttr.optional,
                                    "Unable to find field with type '" + info.FieldType +
                                    "' when injecting dependencies into '" + injectable + "'");

                        info.SetValue(injectable, valueObj);
                    }
                    else if (member is PropertyInfo)
                    {
                        var info = member as PropertyInfo;
                        var valueObj = ResolveField(info.PropertyType, context);

                        ZenUtil.Assert(valueObj != null || injectAttr.optional,
                                "Unable to find property with type '" + info.PropertyType +
                                "' when injecting dependencies into '" + injectable + "'");

                        info.SetValue(injectable, valueObj, null);
                    }
                    else
                    {
                        ZenUtil.Assert(false);
                    }
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
