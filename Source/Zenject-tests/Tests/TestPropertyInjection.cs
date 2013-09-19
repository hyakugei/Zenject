using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestPropertyInjection : TestWithContainer
    {
        private class Test1
        {
        }

        private class Test2
        {
            [Inject] public Test1 val1 = null;

            [Inject]
            public Test1 val2 { get; set; }

            [Inject] private Test1 val3 = null;

            [Inject]
            private Test1 val4 { get; set; }

            public Test1 GetVal3()
            {
                return val3;
            }

            public Test1 GetVal4()
            {
                return val4;
            }
        }

        [Test]
        public void TestCase1()
        {
            _container.Bind<Test2>().AsSingle();
            _container.Bind<Test1>().AsSingle();

            var test1 = _container.Resolve<Test2>();

            Assert.That(test1.val1 != null);
            Assert.That(test1.val2 != null);
            Assert.That(test1.GetVal3() != null);
            Assert.That(test1.GetVal4() != null);
        }
    }
}


