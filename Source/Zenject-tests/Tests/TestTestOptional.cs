using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestTestOptional : TestWithContainer
    {
        private class Test1
        {
        }

        private class Test2
        {
            [Inject] public Test1 val1 = null;
        }

        private class Test3
        {
            [Inject(InjectFlag.Optional)] public Test1 val1 = null;
        }

        [Test]
        [ExpectedException]
        public void TestRequired()
        {
            _container.Bind<Test2>().AsSingle();

            var test1 = _container.Resolve<Test2>();
        }

        [Test]
        public void TestOptional()
        {
            _container.Bind<Test3>().AsSingle();

            var test = _container.Resolve<Test3>();

            Assert.That(test.val1 == null);
        }
    }
}


