using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;

namespace StorEvil.VS2010
{
    /// <summary>
    /// Classifier that classifies all text as an instance of the OrinaryClassifierType
    /// </summary>
    class StorEvilClassifier : IClassifier
    {
        IClassificationType _pending;
        private IClassificationType _comment;
        private IClassificationType _interpreted;
        private IClassificationType _table;
        private IClassificationType _scenarioStart;

        private static int count = 0;
        internal StorEvilClassifier(IClassificationTypeRegistryService registry)
        {
            _pending = registry.GetClassificationType("StorEvil.VS2010.Pending");
            _comment = registry.GetClassificationType("StorEvil.VS2010.Comment");
            _interpreted = registry.GetClassificationType("StorEvil.VS2010.Interpreted");
            _scenarioStart = registry.GetClassificationType("StorEvil.VS2010.ScenarioStart");
            _table = registry.GetClassificationType("StorEvil.VS2010.Table");
        }

        /// <summary>
        /// This method scans the given SnapshotSpan for potential matches for this classification.
        /// In this instance, it classifies everything and returns each span as a new ClassificationSpan.
        /// </summary>
        /// <param name="trackingSpan">The span currently being classified</param>
        /// <returns>A list of ClassificationSpans that represent spans identified to be of this classification</returns>
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            //create a list to hold the results
            List<ClassificationSpan> classifications = new List<ClassificationSpan>();

            string msg = "Start:" + span.Start + " End:" + span.End + "\r\n" + span.GetText() + "\r\n";
            Debug.WriteLine(msg);

            var classificationType = _pending;
            if (IsScenarioStart(span))
                classificationType = _scenarioStart;
            else if (IsComment(span))
                classificationType = _comment;
            else if (IsTable(span))
                classificationType = _table;
            else if (count++ % 2 == 0)
                classificationType = _interpreted;

            classifications.Add(new ClassificationSpan(new SnapshotSpan(span.Snapshot, new Span(span.Start, span.Length)),
                                                           classificationType));
            return classifications;
        }

        private bool IsTable(SnapshotSpan span)
        {
            var trimmed = span.GetText().Trim();

            return trimmed.StartsWith("|") && trimmed.EndsWith("|");
        }

        private bool IsScenarioStart(SnapshotSpan span)
        {
            var start = span.GetText().TrimStart();
            return start.StartsWith("Scenario:") || start.StartsWith("Scenario Outline:");
        }

        private bool IsComment(SnapshotSpan span)
        {
            return span.GetText().TrimStart().StartsWith("#");
        }

#pragma warning disable 67
        // This event gets raised if a non-text change would affect the classification in some way,
        // for example typing /* would cause the classification to change in C# without directly
        // affecting the span.
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67
    }
}
