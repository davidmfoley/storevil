using System;
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
            return new SimpleParameterMatchWordFilter(parameterInfo);
        }

        public WordFilter GetTextFilter(string word)
        {
            return new TextMatchWordFilter(word);
        }
    }

    public class EnumParameterWordFilter : ParameterMatchWordFilter
    {
        private readonly ContextMemberNameSplitter _nameSplitter = new ContextMemberNameSplitter();

        public EnumParameterWordFilter(ParameterInfo parameterInfo) : base(parameterInfo)
        {
        }

        public override WordMatch GetMatch(string[] s)
        {
            var enumValues = Enum.GetValues(ParamInfo.ParameterType);
            var joined = string.Join("", s).ToLower();
            foreach (var enumValue in enumValues)
            {
                if (IsMatch(enumValue, s))
                    return BuildWordMatch(enumValue.ToString(), s);
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

        public bool IsMultipleWordMatcher
        {
            get { return ParamInfo.GetCustomAttributes(typeof (MultipleWordsAttribute), false).Any(); }
        }

        public override WordMatch GetMatch(string[] s)
        {
            return new WordMatch(1, s[0]);
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

        public abstract WordMatch GetMatch(string[] s);
    }
}