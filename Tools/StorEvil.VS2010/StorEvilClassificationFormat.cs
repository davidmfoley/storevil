using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace StorEvil.VS2010
{
    
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "StorEvil.VS2010.Pending")]
    [Name("StorEvil.VS2010.Pending")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class StorEvilClassificationFormatPending : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "StorEvil.VS2010" classification type
        /// </summary>
        public StorEvilClassificationFormatPending()
        {
            this.DisplayName = "StorEvil.VS2010.Pending"; //human readable version of the name
            this.ForegroundColor = Colors.BlueViolet;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "StorEvil.VS2010.Comment")]
    [Name("StorEvil.VS2010.Comment")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class StorEvilClassificationFormatComment : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "StorEvil.VS2010" classification type
        /// </summary>
        public StorEvilClassificationFormatComment()
        {
            this.DisplayName = "StorEvil.VS2010.Comment"; //human readable version of the name
            this.ForegroundColor = Colors.Green;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "StorEvil.VS2010.ScenarioStart")]
    [Name("StorEvil.VS2010.ScenarioStart")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class StorEvilClassificationFormatScenarioStart : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "StorEvil.VS2010" classification type
        /// </summary>
        public StorEvilClassificationFormatScenarioStart()
        {
            this.DisplayName = "StorEvil.VS2010.ScenarioStart"; //human readable version of the name
            this.ForegroundColor = Colors.Blue;            
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "StorEvil.VS2010.Table")]
    [Name("StorEvil.VS2010.Table")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class StorEvilClassificationFormatTable : ClassificationFormatDefinition
    {
        /// <summary>
        /// Defines the visual format for the "StorEvil.VS2010" classification type
        /// </summary>
        public StorEvilClassificationFormatTable()
        {
            this.DisplayName = "StorEvil.VS2010.Table"; //human readable version of the name
            this.ForegroundColor = Colors.Gray;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "StorEvil.VS2010.Interpreted")]
    [Name("StorEvil.VS2010.Interpreted")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class StorEvilClassificationFormatInterpreted : ClassificationFormatDefinition
    {
        public StorEvilClassificationFormatInterpreted()
        {
            this.DisplayName = "StorEvil.VS2010,Interpreted"; //human readable version of the name
            this.ForegroundColor = Colors.Black;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }
}
