using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestSingleton : TestWithContainer
    {
        private interface ITest
        {
            int ReturnValue();
        }

        private class Test : ITest
        {
            public int ReturnValue()
            {
                return 5;
            }
        }

        [Test]
        public void TestClassRegistration()
        {
            _container.Bind<Test>().AsSingle();

            Assert.That(_container.Resolve<Test>().ReturnValue() == 5);
        }

        [Test]
        public void TestSingletonOneInstance()
        {
            _container.Bind<Test>().AsSingle();

            var test1 = _container.Resolve<Test>();
            var test2 = _container.Resolve<Test>();

            Assert.That(test1 != null && test2 != null);
            Assert.That(ReferenceEquals(test1, test2));
        }

        [Test]
        public void TestInterfaceBoundToImplementationRegistration()
        {
            _container.Bind<ITest>().AsSingle<Test>();

            Assert.That(_container.Resolve<ITest>().ReturnValue() == 5);
        }

        [Test]
        public void TestInterfaceBoundToInstanceRegistration()
        {
            ITest instance = new Test();

            _container.Bind<ITest>().AsSingle(instance);

            var builtInstance = _container.Resolve<ITest>();

            Assert.That(ReferenceEquals(builtInstance, instance));
            Assert.That(builtInstance.ReturnValue() == 5);
        }

        [Test]
        public void TestDuplicateBindings()
        {
            // Note: does not error out until a request for Test is made
            _container.Bind<Test>().AsSingle();
            _container.Bind<Test>().AsSingle();
        }

        [Test]
        [ExpectedException]
        public void TestDuplicateBindingsFail()
        {
            _container.Bind<Test>().AsSingle();
            _container.Bind<Test>().AsSingle();

            var test1 = _container.Resolve<Test>();
        }

        [Test]
        [ExpectedException]
        public void TestDuplicateInstanceBindingsFail()
        {
            var instance = new Test();

            _container.Bind<Test>().AsSingle(instance);
            _container.Bind<Test>().AsSingle(instance);

            var test1 = _container.Resolve<Test>();
        }
    }
}


