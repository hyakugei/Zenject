using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace ModestTree.Zenject.Test
{
    public class TestBase
    {
        [SetUp]
        public virtual void Setup()
        {
            // Throw exceptions so we can use the ExpectedException attribute to test wrong usage
            Util.SetAssertHandleMethod(AssertHandleMethod.Exception);
        }

        [TearDown]
        public virtual void Destroy()
        {
            Util.SetAssertHandleMethod(AssertHandleMethod.LogAndContinue);
        }
    }
}
