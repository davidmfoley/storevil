using System;

namespace StorEvil.Context
{
    [Serializable]
    public class StorEvilException : Exception
    {
        public StorEvilException(string s) : base(s)
        {
            
        }
    }
}