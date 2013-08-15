using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Zenject.Test
{
    public class TestExceptionsWithContainer : TestWithContainer
    {
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            ZenUtil.SetAssertHandleMethod(AssertHandleMethod.Exception);
        }

        [TearDown]
        public override void Destroy()
        {
            base.Destroy();
            ZenUtil.SetAssertHandleMethod(AssertHandleMethod.LogAndContinue);
        }
    }

}
