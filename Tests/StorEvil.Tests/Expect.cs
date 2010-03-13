using System;
using NUnit.Framework;

namespace StorEvil
{
    public static class Expect
    {
        public static T ThisToThrow<T>(Action action) where T : Exception
        {
            try
            {
                action();
            }
            catch (T ex)
            {
                return ex;
            }
            catch (Exception exception)
            {
                Assert.Fail("Unexpected exception thrown: expected " + typeof(T).Name + " but caught " +
                            exception.GetType().Name + "\n" + exception);
            }
            Assert.Fail("Expected exception " + typeof(T).Name + " was not thrown.");

            return null;
        }
    }
}