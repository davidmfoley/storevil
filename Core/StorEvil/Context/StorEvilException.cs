using System;
using System.Runtime.Serialization;

namespace StorEvil
{
    [Serializable]
    public class StorEvilException : Exception
    {       
        public StorEvilException()
        {
        }

        public StorEvilException(string message) : base(message)
        {
        }

        public StorEvilException(string message, Exception inner) : base(message, inner)
        {
        }

        protected StorEvilException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }

    [Serializable]
    public class AssertionException : StorEvilException
    {
        public AssertionException()
        {
        }

        public AssertionException(string message)
            : base(message)
        {
        }

        public AssertionException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected AssertionException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }


    [Serializable]
    public class IgnoreException : StorEvilException
    {
        public IgnoreException()
        {
        }

        public IgnoreException(string message)
            : base(message)
        {
        }

        public IgnoreException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected IgnoreException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }
}