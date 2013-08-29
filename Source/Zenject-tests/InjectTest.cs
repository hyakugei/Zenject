using System;
using System.Collections.Generic;
using ModestTree.Zenject;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    [TestFixture]
    public class TestSingleton1 : TestWithContainer
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
    }

    [TestFixture]
    public class TestSingleton2 : TestWithContainer
    {
        private class Test
        {
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

    [TestFixture]
    public class TestTestOptional : TestWithContainer
    {
        private class Test1
        {
        }

        private class Test2
        {
#pragma warning disable 649
            [Inject] public Test1 val1;
#pragma warning restore 649
        }

        private class Test3
        {
#pragma warning disable 649
            [Inject(InjectFlag.Optional)] public Test1 val1;
#pragma warning restore 649
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

    [TestFixture]
    public class TestPropertyInjection : TestWithContainer
    {
        private class Test1
        {
        }

        private class Test2
        {
#pragma warning disable 649
            [Inject] public Test1 val1;

            [Inject]
            public Test1 val2 { get; set; }

            [Inject] private Test1 val3;
#pragma warning disable 649

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

    [TestFixture]
    public class Test3 : TestWithContainer
    {
        private class Test1
        {
        }

        private class Test2
        {
            public Test1 val;

            public Test2(Test1 val)
            {
                this.val = val;
            }
        }

        [Test]
        public void TestConstructorInjection()
        {
            _container.Bind<Test2>().AsSingle();
            _container.Bind<Test1>().AsSingle();

            var test1 = _container.Resolve<Test2>();

            Assert.That(test1.val != null);
        }

        [Test]
        public void TestConstructByFactory()
        {
            _container.Bind<Test2>().AsSingle();

            var val = new Test1();
            var factory = new Factory<Test2>(_container);
            var test1 = factory.Create(val);

            Assert.That(test1.val == val);
        }
    }

    [TestFixture]
    public class TestTransientInjection : TestWithContainer
    {
        private class Test1
        {
        }

        [Test]
        public void TestTransientType()
        {
            _container.Bind<Test1>().AsTransient();

            var test1 = _container.Resolve<Test1>();
            var test2 = _container.Resolve<Test1>();

            Assert.That(test1 != null && test2 != null);
            Assert.That(!ReferenceEquals(test1, test2));
        }
    }

    [TestFixture]
    public class Test6 : TestWithContainer
    {
        private class Test1
        {
            public int f1;
            public int f2;

            public Test1(int f1, int f2)
            {
                this.f1 = f1;
                this.f2 = f2;
            }
        }

        [Test]
        public void TestExtraParametersSameType()
        {
            var factory1 = new Factory<Test1>(_container);
            var test1 = factory1.Create(5, 10);

            Assert.That(test1 != null);
            Assert.That(test1.f1 == 5 && test1.f2 == 10);

            var factory2 = new Factory<Test1>(_container);
            var test2 = factory2.Create(10, 5);

            Assert.That(test2 != null);
            Assert.That(test2.f1 == 10 && test2.f2 == 5);
        }

        [Test]
        [ExpectedException]
        public void TestMissingParameterThrows()
        {
            _container.Bind<Test1>().AsTransient();
            _container.Resolve<Test1>();
        }
    }

    [TestFixture]
    public class Test7 : TestWithContainer
    {
        private class Test0
        {
        }
        private class Test3
        {
        }

        private class Test1 : Test3
        {
            [Inject] protected Test0 val;

            public Test0 GetVal()
            {
                return val;
            }
        }

        private class Test2 : Test1
        {
        }

        [Test]
        public void TestBaseClassPropertyInjection()
        {
            _container.Bind<Test0>().AsSingle();
            _container.Bind<Test2>().AsSingle();

            var test1 = _container.Resolve<Test2>();

            Assert.That(test1.GetVal() != null);
        }
    }

    [TestFixture]
    public class Test8 : TestWithContainer
    {
        private class Test0
        {
        }

        private class Test1
        {
            public Test1(Test0 test1)
            {
            }
        }

        [Test]
        [ExpectedException]
        public void TestDuplicateInjection()
        {
            _container.Bind<Test0>().AsSingle();
            _container.Bind<Test0>().AsSingle();

            _container.Bind<Test1>().AsSingle();

            _container.Resolve<Test1>();
        }
    }

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

    [TestFixture]
    public class Test10 : TestWithContainer
    {
        private class Test0
        {
        }

        [Test]
        public void TestCustomScope()
        {
            using (var scope = new CustomScope(_container))
            {
                var test1 = new Test0();

                scope.Bind<Test0>().AsSingle(test1);

                Assert.That(ReferenceEquals(test1, _container.Resolve<Test0>()));
            }

            Assert.That(_container.Resolve<Test0>() == null);
        }
    }

    [TestFixture]
    public class Test11 : TestWithContainer
    {
        private class Test1
        {
        }

        private class Test2 : Test1
        {
        }

        private class Test3 : Test1
        {
        }

        private class TestImpl1
        {
            public List<Test1> tests;

            public TestImpl1(List<Test1> tests)
            {
                this.tests = tests;
            }
        }

        private class TestImpl2
        {
            [Inject]
            public List<Test1> tests;
        }

        [Test]
        public void TestMultiBind()
        {
            _container.Bind<Test1>().AsSingle<Test2>();
            _container.Bind<Test1>().AsSingle<Test3>();
            _container.Bind<TestImpl1>().AsSingle();

            var test1 = _container.Resolve<TestImpl1>();

            Assert.That(test1.tests.Count == 2);
        }

        [Test]
        public void TestMultiBind2()
        {
            _container.Bind<TestImpl1>().AsSingle();

            var test1 = _container.Resolve<TestImpl1>();

            // Allow for zero
            Assert.That(test1.tests.Count == 0);
        }

        [Test]
        public void TestMultiBindListInjection()
        {
            _container.Bind<Test1>().AsSingle<Test2>();
            _container.Bind<Test1>().AsSingle<Test3>();
            _container.Bind<TestImpl2>().AsSingle();

            var test = _container.Resolve<TestImpl2>();
            Assert.That(test.tests.Count == 2);
        }
    }

    [TestFixture]
    public class Test12 : TestWithContainer
    {
        private class Test0
        {
        }

        private class Test3 : Test0
        {
        }

        private class Test4 : Test0
        {
        }

        private class Test2
        {
            public Test0 test;

            public Test2(Test0 test)
            {
                this.test = test;
            }
        }

        private class Test1
        {
            public List<Test0> test;

            public Test1(List<Test0> test)
            {
                this.test = test;
            }
        }

        [Test]
        [ExpectedException]
        public void TestMultiBind2()
        {
            // Multi-binds should not map to single-binds
            _container.Bind<Test0>().AsSingle<Test3>();
            _container.Bind<Test0>().AsSingle<Test4>();
            _container.Bind<Test2>().AsSingle();

            _container.Resolve<Test2>();
        }
    }

    [TestFixture]
    public class TestConditionsTarget : TestWithContainer
    {
        private class Test0
        {
        }

        private class Test1
        {
            public Test1(Test0 test)
            {
            }
        }

        private class Test2
        {
            public Test2(Test0 test)
            {
            }
        }

        public override void Setup()
        {
            base.Setup();
            _container.Bind<Test0>().AsSingle().When(r => r.target == typeof (Test2));
        }

        [Test]
        [ExpectedException]
        public void TestTargetConditionError()
        {
            _container.Bind<Test1>().AsSingle();
            _container.Resolve<Test1>();
        }

        [Test]
        public void TestTargetConditionSuccess()
        {
            _container.Bind<Test2>().AsSingle();
            var test2 = _container.Resolve<Test2>();

            Assert.That(test2 != null);
        }
   }

    [TestFixture]
    public class TestConditionsName : TestWithContainer
    {
        private class Test0
        {

        }

        private class Test1
        {
            public Test1(Test0 name1)
            {
            }
        }

        private class Test2
        {
            public Test2(Test0 name2)
            {
            }
        }

        public override void Setup()
        {
            base.Setup();
            _container.Bind<Test0>().AsSingle().When(r => r.name == "name1");
        }

        [Test]
        [ExpectedException]
        public void TestNameConditionError()
        {
            _container.Bind<Test2>().AsSingle();
            _container.Resolve<Test2>();
        }

        [Test]
        public void TestNameConditionSuccess()
        {
            _container.Bind<Test1>().AsSingle();
            var test1 = _container.Resolve<Test1>();

            Assert.That(test1 != null);
        }
    }

    [TestFixture]
    public class TestConditionsParents : TestWithContainer
    {
        class Test0
        {
        }

        class Test1
        {
            public Test0 test0;

            public Test1(Test0 test0)
            {
                this.test0 = test0;
            }
        }

        class Test2
        {
            public Test0 test0;

            public Test2(Test0 test0)
            {
                this.test0 = test0;
            }
        }

        class Test3
        {
            public Test1 test1;

            public Test3(Test1 test1)
            {
                this.test1 = test1;
            }
        }

        class Test4
        {
            public Test1 test1;

            public Test4(Test1 test1)
            {
                this.test1 = test1;
            }
        }

        [Test]
        [ExpectedException]
        public void TestCase1()
        {
            _container.Bind<Test1>().AsSingle();
            _container.Bind<Test0>().AsSingle().When(c => c.parents.Contains(typeof(Test2)));

            _container.Resolve<Test1>();
        }

        [Test]
        public void TestCase2()
        {
            _container.Bind<Test1>().AsSingle();
            _container.Bind<Test0>().AsSingle().When(c => c.parents.Contains(typeof(Test1)));

            var test1 = _container.Resolve<Test1>();
            Assert.That(test1 != null);
        }

        [Test]
        // Test using parents to look deeper up the heirarchy..
        public void TestCase3()
        {
            var t0a = new Test0();
            var t0b = new Test0();

            _container.Bind<Test3>().AsSingle();
            _container.Bind<Test4>().AsSingle();
            _container.Bind<Test1>().AsTransient();

            _container.Bind<Test0>().AsSingle(t0a).When(c => c.parents.Contains(typeof(Test3)));
            _container.Bind<Test0>().AsSingle(t0b).When(c => c.parents.Contains(typeof(Test4)));

            var test3 = _container.Resolve<Test3>();
            var test4 = _container.Resolve<Test4>();

            Assert.That(ReferenceEquals(test3.test1.test0, t0a));
            Assert.That(ReferenceEquals(test4.test1.test0, t0b));
        }
    }

    [TestFixture]
    public class TestMultipleInterfaceSameSingle : TestWithContainer
    {
        private interface ITest1
        {
        }

        private interface ITest2
        {
        }

        private class Test1 : ITest1, ITest2
        {
        }

        [Test]
        public void TestCase1()
        {
            _container.Bind<ITest1>().AsSingle<Test1>();
            _container.Bind<ITest2>().AsSingle<Test1>();

            var test1 = _container.Resolve<ITest1>();
            var test2 = _container.Resolve<ITest2>();

            Assert.That(ReferenceEquals(test1, test2));
        }
    }

    [TestFixture]
    public class TestResolveMany : TestWithContainer
    {
        private class Test0
        {
        }

        private class Test1 : Test0
        {
        }

        private class Test2 : Test0
        {
        }

        [Test]
        public void TestCase1()
        {
            _container.Bind<Test0>().AsSingle<Test1>();
            _container.Bind<Test0>().AsSingle<Test2>();

            List<Test0> many = _container.ResolveMany<Test0>();

            Assert.That(many.Count == 2);
        }

        [Test]
        public void TestCase2()
        {
            List<Test0> many = _container.ResolveMany<Test0>();

            Assert.That(many.Count == 0);
        }
    }

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

    [TestFixture]
    public class TestCustomScope : TestWithContainer
    {
        private class Test1
        {
        }

        [Test]
        public void TestRelease1()
        {
            _container.Bind<Test1>().AsSingle();
            _container.Release<Test1>();

            var test1 = _container.Resolve<Test1>();
            Assert.That(test1 == null);

            _container.Bind<Test1>().AsSingle();

            test1 = _container.Resolve<Test1>();
            Assert.That(test1 != null);
        }
    }

    [TestFixture]
    public class TestCustomScope2 : TestWithContainer
    {
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

    [TestFixture]
    public class TestCircularDependencies : TestWithContainer
    {
        class Test1
        {
            [Inject]
            public Test2 test;
        }

        class Test2
        {
            [Inject]
            public Test2 test;
        }

        [Test]
        [ExpectedException]
        public void Test()
        {
            _container.Bind<Test1>().AsSingle();
            _container.Bind<Test2>().AsSingle();

            var test = _container.Resolve<Test2>();
        }
    }

    [TestFixture]
    public class TestSameConstructorArgumentType : TestWithContainer
    {
        class Test1
        {
            public Test2 t1;
            public float f;
            public Test2 t2;

            public Test1(Test2 t1, float f, Test2 t2)
            {
                this.t1 = t1;
                this.f = f;
                this.t2 = t2;
            }
        }

        class Test2
        {
        }

        [Test]
        public void Test()
        {
            var t1 = new Test2();
            var t2 = new Test2();

            _container.Bind<Factory<Test1>>().AsSingle();

            var factory = _container.Resolve<Factory<Test1>>();

            var test = factory.Create(t1, 5.0f, t2);

            Assert.That(ReferenceEquals(test.t1, t1));
            Assert.That(ReferenceEquals(test.t2, t2));
        }
    }

    [TestFixture]
    public class TestGetDependencies : TestWithContainer
    {
        class Test5
        {
        }

        class Test4
        {
        }

        class Test3
        {
            public Test3(Test4 test, Test5 test2)
            {
            }
        }

        class Test1
        {
        }

        class Test2
        {
            public Test2(Test1 test1, Test3 test3)
            {
            }
        }

        [Test]
        public void Test()
        {
            _container.Bind<Test1>().AsSingle();
            _container.Bind<Test2>().AsSingle();
            _container.Bind<Test3>().AsSingle();
            _container.Bind<Test4>().AsSingle();
            _container.Bind<Test5>().AsSingle();

            var deps = _container.CalculateObjectGraph<Test2>();

            Assert.That(deps[typeof(Test2)].Count == 2);
            Assert.That(deps[typeof(Test1)].Count == 0);
            Assert.That(deps[typeof(Test3)].Count == 2);
        }
    }
}
