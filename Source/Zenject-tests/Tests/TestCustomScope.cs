using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestCustomScope : TestWithContainer
    {
        private class Test0
        {
        }

        interface Test1
        {
        }

        interface Test2
        {
        }

        private class Test3 : Test1, Test2
        {
        }

        [Test]
        public void TestCase1()
        {
            using (var scope = new CustomScope(_container))
            {
                var test1 = new Test0();

                scope.Bind<Test0>().AsSingle(test1);

                Assert.That(ReferenceEquals(test1, _container.Resolve<Test0>()));
            }

            Assert.That(_container.Resolve<Test0>() == null);
        }

        [Test]
        public void TestRelease1()
        {
            _container.Bind<Test0>().AsSingle();
            _container.Release<Test0>();

            var test1 = _container.Resolve<Test0>();
            Assert.That(test1 == null);

            _container.Bind<Test0>().AsSingle();

            test1 = _container.Resolve<Test0>();
            Assert.That(test1 != null);
        }

        [Test]
        public void TestRelease()
        {
            _container.Bind<Test1>().AsSingle<Test3>();
            _container.Bind<Test2>().AsSingle<Test3>();

            var test1 = _container.Resolve<Test1>();
            Assert.That(test1 != null);

            _container.Release<Test1>();

            test1 = _container.Resolve<Test1>();
            Assert.That(test1 == null);

            var test2 = _container.Resolve<Test2>();
            Assert.That(test2 != null);

            _container.Release<Test2>();

            test2 = _container.Resolve<Test2>();
            Assert.That(test2 == null);
        }
    }
}


