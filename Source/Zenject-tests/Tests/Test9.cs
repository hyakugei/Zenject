using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class Test9 : TestWithContainer
    {
        private abstract class Test0
        {
        }

        private class Test1 : Test0
        {
        }

        [Test]
        public void TestBothInterfaceAndConcreteBoundToSameSingleton()
        {
            _container.Bind<Test0>().AsSingle<Test1>();
            _container.Bind<Test1>().AsSingle();

            var test1 = _container.Resolve<Test0>();
            var test2 = _container.Resolve<Test1>();

            Assert.That(ReferenceEquals(test1, test2));
        }
    }
}


