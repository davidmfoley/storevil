using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StorEvil.Context.Matchers;

namespace StorEvil.Context.WordFilters
{
    public class WordFilterFactory
    {
        public WordFilter GetParameterFilter(ParameterInfo parameterInfo)
        {
            if (parameterInfo.ParameterType.IsEnum)
                return new EnumParameterWordFilter(parameterInfo);

            if (parameterInfo.GetCustomAttributes(typeof(MultipleWordsAttribute), false).Any())
                return new MultipleWordsParameterWordFilter(parameterInfo);
            return new SimpleParameterMatchWordFilter(parameterInfo);
        }

        public WordFilter GetTextFilter(string word)
        {
            return new TextMatchWordFilter(word);
        }
    }

    public class MultipleWordsParameterWordFilter : ParameterMatchWordFilter
    {
        public MultipleWordsParameterWordFilter(ParameterInfo paramInfo)
            : base(paramInfo)
        {
        }

        public bool IsTable
        {
            get { return ParamInfo.ParameterType.IsArray; }
        }

        public override IEnumerable<WordMatch> GetMatches(string[] s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                yield return new WordMatch(s.Length - i, string.Join(" ", s.Take(s.Length - i).ToArray()));
            }           
        }
    }

    public class EnumParameterWordFilter : ParameterMatchWordFilter
    {
        private readonly ContextMemberNameSplitter _nameSplitter = new ContextMemberNameSplitter();

        public EnumParameterWordFilter(ParameterInfo parameterInfo) : base(parameterInfo)
        {
        }

        public override IEnumerable<WordMatch> GetMatches(string[] s)
        {
            var enumValues = Enum.GetValues(ParamInfo.ParameterType);
            
            foreach (var enumValue in enumValues)
            {
                if (IsMatch(enumValue, s))
                    return new[] { BuildWordMatch(enumValue.ToString(), s)};
            }

            return WordMatch.NoMatch();
        }

        private bool IsMatch(object enumValue, string[] s)
        {
            var enumValueWords = _nameSplitter.SplitMemberName(enumValue.ToString()).ToArray();
            if (s.Length < enumValueWords.Length)
                return false;

            for (int i = 0; i < enumValueWords.Length; i++)
            {
                if (s[i].ToLower() != enumValueWords[i].ToLower())
                    return false;
            }

            return true;
        }

        private WordMatch BuildWordMatch(string enumValue, string[] words)
        {
            var wordIndex = 0;
            var length = words[0].Length;
            while (enumValue.Length > length)
                length += words[++wordIndex].Length;
            return new WordMatch(wordIndex + 1, string.Join("", words.Take(wordIndex + 1).ToArray()));
        }
    }

    /// <summary>
    /// word filter that matches parameters
    /// </summary>
    public class SimpleParameterMatchWordFilter : ParameterMatchWordFilter
    {
        public SimpleParameterMatchWordFilter(ParameterInfo paramInfo) : base(paramInfo)
        {
        }

        public bool IsTable
        {
            get { return ParamInfo.ParameterType.IsArray; }
        }

       
        public override IEnumerable<WordMatch> GetMatches(string[] s)
        {
            return new[] { new WordMatch(1, s[0])};
        }

        private bool IsTableString(string paramValue)
        {
            return paramValue.StartsWith("|");
        }
    }

    /// <summary>
    /// word filter that matches parameters
    /// </summary>
    public abstract class ParameterMatchWordFilter : WordFilter
    {
        protected readonly ParameterInfo ParamInfo;

        public ParameterMatchWordFilter(ParameterInfo paramInfo)
        {
            ParamInfo = paramInfo;
        }

        public string ParameterName
        {
            get { return ParamInfo.Name; }
        }

        public abstract IEnumerable<WordMatch> GetMatches(string[] s);
    }
}