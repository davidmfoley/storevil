using System;
using System.Collections.Generic;

namespace StorEvil.Console
{
    public class InPlaceSettings 
    {
        public InPlaceSettings()
        {
            Tags = new string[0];
        }

        public IEnumerable<string> Tags
        {
            get; set;
        }
    }
}