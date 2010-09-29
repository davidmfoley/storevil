using System;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace StorEvil.VS2010
{
    /// <summary>
    /// This class causes a classifier to be added to the set of classifiers. Since 
    /// the content type is set to "text", this classifier applies to all text files
    /// </summary>
    [Export(typeof(IClassifierProvider))]
    [ContentType("text")]
    internal class StorEvilClassifierProvider : IClassifierProvider
    {
        /// <summary>
        /// Import the classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// </summary>
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null; // Set via MEF

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            if (IsStorEvilFile(buffer))
            {
                var factory = buffer.Properties.GetOrCreateSingletonProperty<StorEvilClassifierFactory>(
                    () => new StorEvilClassifierFactory(ClassificationRegistry));
                return factory.GetClassifier(buffer);
            }
            return null;
        }

        private bool IsStorEvilFile(ITextBuffer buffer)
        {
            //temp
            return buffer.CurrentSnapshot.GetText().Contains("Scenario:");
        }
    }

    internal class StorEvilClassifierFactory {
        private readonly IClassificationTypeRegistryService _classificationRegistry;

        public StorEvilClassifierFactory(IClassificationTypeRegistryService classificationRegistry)
        {
            _classificationRegistry = classificationRegistry;
           
        }

        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            return new StorEvilClassifier(_classificationRegistry);
        }
    }
}