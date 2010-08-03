using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace StorEvil.VS2010
{
    /// <summary>
    /// Defines an editor format for the StorEvil.VS2010 type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "StorEvil.VS2010")]
    [Name("StorEvil.VS2010")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class StorEvilClassificationFormat : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "StorEvil.VS2010" classification type
        /// </summary>
        public StorEvilClassificationFormat()
        {
            this.DisplayName = "StorEvil.VS2010"; //human readable version of the name
            this.BackgroundColor = Colors.BlueViolet;
            this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }
}
