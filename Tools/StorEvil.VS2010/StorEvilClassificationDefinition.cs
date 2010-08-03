using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace StorEvil.VS2010
{
    internal static class StorEvilClassificationDefinition
    {
        /// <summary>
        /// Defines the "StorEvil.VS2010" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("StorEvil.VS2010")]
        internal static ClassificationTypeDefinition StorEvilVS2010Type = null;
    }
}
