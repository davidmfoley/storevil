using System.Collections.Generic;
using StorEvil.Core;

namespace StorEvil.Glossary
{
    public class StepViewModel
    {
        public StepDescription Description;
        public bool IsExtensionMethod;

        public string TypeName;

        public string MemberName;

        public IEnumerable<StepViewModel> Children { get; set; }
    }
}