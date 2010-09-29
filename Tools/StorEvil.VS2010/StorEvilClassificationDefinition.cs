using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace StorEvil.VS2010
{
    internal static class StorEvilClassificationDefinition
    {
        
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("StorEvil.VS2010.Pending")]
        internal static ClassificationTypeDefinition StorEvilVS2010PendingType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("StorEvil.VS2010.Interpreted")]
        internal static ClassificationTypeDefinition StorEvilVS2010InterpretedType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("StorEvil.VS2010.Comment")]
        internal static ClassificationTypeDefinition StorEvilVS2010CommentType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("StorEvil.VS2010.ScenarioStart")]
        internal static ClassificationTypeDefinition StorEvilVS2010ScenarioStartType = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name("StorEvil.VS2010.Table")]
        internal static ClassificationTypeDefinition StorEvilVS2010TableType = null;
    }
}
