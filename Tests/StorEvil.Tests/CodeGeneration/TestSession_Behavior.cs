using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using StorEvil.Context;

namespace StorEvil.CodeGeneration
{
    [TestFixture]
    public class TestSession_Behavior
    {
        [SetUp]
        public void SetUpContext()
        {
            TestSession.TestSessionContextFactory = new FakeContextFactory();
        }
        [Test]
        public void returns_same_context_for_multiple_calls()
        {
            var context = TestSession.SessionContext("foo");
            Assert.That(context, Is.SameAs(TestSession.SessionContext("foo")));
        }

        [Test]
        public void returns_different_context_after_ShutDown()
        {
            var context = TestSession.SessionContext("foo");
            TestSession.ShutDown();

            Assert.That(context, Is.Not.SameAs(TestSession.SessionContext("foo")));
        }

        public class FakeContextFactory : TestSessionContextFactory
        {
            public override SessionContext GetSessionContext(string currentAssemblyLocation, IEnumerable<Assembly> assemblies)
            {
                return new SessionContext();
            }
        }
    }
}