using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using StorEvil.CodeGeneration;

namespace StorEvil.NUnit
{
    [TestFixture]
    public class Invoking_NUnit_via_reflection
    {
     
        [Test]
        public void Can_fail()
        {
            var wrapper = new NUnitAssertWrapper();
            var ex = Expect.ThisToThrow<Exception>(()=>wrapper.Fail("Foo"));
            Assert.That(ex.GetType().Name, Is.EqualTo("AssertionException"));
        }

        [Test]
        public void Can_ignore()
        {
            var wrapper = new NUnitAssertWrapper();
            var ex = Expect.ThisToThrow<Exception>(() => wrapper.Ignore("Foo"));
            Assert.That(ex.GetType().Name, Is.EqualTo("IgnoreException"));
        }

        [Test]
        public void when_failing_and_nunit_assembly_cannot_be_found_falls_back_to_StorEvil_AssertionException()
        {
            var wrapper = new NUnitAssertWrapper("fake.assembly.name");
            Expect.ThisToThrow<StorEvil.AssertionException>(() => wrapper.Fail("Foo"));
       
        }

        [Test]
        public void when_failing_and_nunit_assembly_cannot_be_found_falls_back_to_StorEvil_IgnoreException()
        {
            var wrapper = new NUnitAssertWrapper("fake.assembly.name");
            Expect.ThisToThrow<StorEvil.IgnoreException>(() => wrapper.Ignore("Foo"));

        }
    }
}