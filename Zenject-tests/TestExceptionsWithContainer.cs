using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    public class TestExceptionsWithContainer : TestWithContainer
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            Util.SetAssertHandleMethod(AssertHandleMethod.Exception);
        }

        [TearDown]
        public override void Destroy()
        {
            base.Destroy();
            Util.SetAssertHandleMethod(AssertHandleMethod.LogAndContinue);
        }
    }

}
