using System.Collections.Generic;
using System.Linq;
using StorEvil.Context.WordFilters;

namespace StorEvil.Core
{
    internal class WordFilterDescriber
    {     
        public StepDescription Describe(IEnumerable<WordFilter> filters)
        {
            if (filters.Count() > 1)
            {
                var first = TranslateWordFilter(filters.First());
                var last = TranslateWordFilter(filters.Last());
                var middle = filters.Skip(1).Take(filters.Count() - 2).SelectMany(x => TranslateWordFilter(x));

                var spans = ProcessSpans(first.Union(middle).Union(last));

                return new StepDescription {Spans = spans};
            }
            return new StepDescription { };
        }

        private IEnumerable<StepSpan> ProcessSpans(IEnumerable<StepSpan> stepSpans)
        {
            var a = InjectSpaces(stepSpans);
            var joined = JoinAdjacentTextSpans(a);
            return joined;
        }

        private IEnumerable<StepSpan> JoinAdjacentTextSpans(IEnumerable<StepSpan> stepSpans)
        {
            var current = new List<TextSpan>();

            foreach (var stepSpan in stepSpans)
            {
                if (stepSpan is TextSpan)
                    current.Add((TextSpan)stepSpan);
                else
                {
                    if (current.Any())
                    {
                        yield return TextSpan.Merge(current);
                        current.Clear();
                    }
                    yield return stepSpan;
                }
            }

            if (current.Any())
                yield return TextSpan.Merge(current);
        }

        private IEnumerable<StepSpan> InjectSpaces(IEnumerable<StepSpan> joined)
        {
            if (!joined.Any())
                yield break;

            yield return joined.First();

            foreach (var stepSpan in joined.Skip(1))
            {
                yield return new TextSpan(" ");
                yield return stepSpan;
            }
        }

        private IEnumerable<StepSpan> TranslateWordFilter(WordFilter wordFilter)
        {
           
            if (wordFilter is TextMatchWordFilter)
            {
                yield return new TextSpan(((TextMatchWordFilter)wordFilter).Word);
            }
            else if (wordFilter is ParameterMatchWordFilter)
            {
                var paramMatch = ((ParameterMatchWordFilter)wordFilter);
                yield return new ParameterSpan(paramMatch.ParameterType, paramMatch.ParameterName);
            }
            else
            {
                yield return new TextSpan("");
            }
        }
    }
}