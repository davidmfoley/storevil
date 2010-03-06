using System.Reflection;

namespace StorEvil.Context
{
    /// <summary>
    /// word filter that matches parameters
    /// </summary>
    public class ParameterMatchWordFilter : WordFilter
    {
        private readonly ParameterInfo _paramInfo;

        public ParameterMatchWordFilter(ParameterInfo paramInfo)
        {
            _paramInfo = paramInfo;
        }

        public string ParameterName
        {
            get { return _paramInfo.Name; }
        }

        public bool IsMatch(string s)
        {
            return true;
        }
    }
}