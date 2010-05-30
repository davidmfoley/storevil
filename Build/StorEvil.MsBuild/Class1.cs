using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;

namespace StorEvil.MsBuild
{
    public class StorEvilTask : ITask
    {
        public bool Execute()
        {
            return true;
        }
            
        public string Path { get; set; }

        public IBuildEngine BuildEngine
        {
            get; set;
        }

        public ITaskHost HostObject
        {
            get; set;
        }
    }
}
