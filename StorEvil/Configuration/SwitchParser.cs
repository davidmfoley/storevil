using System.Collections.Generic;
using System.Linq;

namespace StorEvil.Core.Configuration
{
    public class SwitchParser<T>
    {
        private readonly List<SwitchInfo<T>> _switchInfos = new List<SwitchInfo<T>>();

        public SwitchInfo<T> AddSwitch(params string[] switches)
        {
            var switchInfo = new SwitchInfo<T>(switches);
            _switchInfos.Add(switchInfo);
            return switchInfo;
        }

        public void Parse(string[] args, T settings)
        {
            var switchParams = new List<string>();

            SwitchInfo<T> currentSwitchInfo = null;

            foreach (var arg in args)
            {
                var s = arg;
                var switchInfo = _switchInfos.FirstOrDefault(x => x.Matches(s));

                if (switchInfo != null)
                {
                    if (currentSwitchInfo != null)
                        currentSwitchInfo.Execute(settings, switchParams.ToArray());

                    switchParams = new List<string>();
                    currentSwitchInfo = switchInfo;
                }
                else
                {
                    switchParams.Add(arg);
                }
            }

            if (currentSwitchInfo != null)
                currentSwitchInfo.Execute(settings, switchParams.ToArray());
        }
    }
}