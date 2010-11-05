using System;
using System.Reflection;
using StorEvil.Context;
using StorEvil.Utility;

namespace StorEvil.NUnit
{
    public class TestContext : IDisposable
    {
        public TestSubContext SubContext
        {
            get { return SubContextField; }
            set { SubContextField = value; }
        }

        public TestSubContext SubContextField = new TestSubContext();

        public virtual TestSubContext Some_Sub_Context()
        {
            return SubContext;
        }

        public virtual void Given_A_User_Named(string userName)
        {
        }

        public virtual void Given_A_Dog_Named(string userName)
        {
        }

        public virtual void A_Condition_With_intParam_and_stringParam(int intParam, string stringParam)
        {
        }

        public virtual void A_Condition_With_dateTime_param(DateTime dateTime)
        {
        }

        public virtual void When_I_Do_Something()
        {
        }

        public virtual void Then_Something_Should_Happen()
        {
        }

        public void Dispose()
        {
        }
    }

    public class TestSubContext
    {
        public string LastParameter { get; set; }

        public virtual void Condition(string parameter)
        {
            LastParameter = parameter;
        }

        public string Name { get; set; }
    }
}