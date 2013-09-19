using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class Test14 : TestWithContainer
    {
        private struct Test1
        {
        }

        private class Test2
        {
            public Test2(Test1 t1)
            {
            }
        }

        [Test]
        public void TestStructInjection()
        {
            _container.Bind<Test1>().AsSingle();
            _container.Bind<Test2>().AsSingle();

            var t2 = _container.Resolve<Test2>();

            Assert.That(t2 != null);
        }
    }
}


