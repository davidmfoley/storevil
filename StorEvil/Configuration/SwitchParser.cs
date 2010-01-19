using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace StorEvil.Core.Configuration
{
    public class SwitchParser<T>
    {
        public SwitchParser()
        {
            var members = typeof (T).GetMembers();
          
            foreach (var info in members)
            {
                var customSwitchAttrs = info.GetCustomAttributes(typeof (CommandSwitchAttribute), true).Cast<CommandSwitchAttribute>();
                if (!customSwitchAttrs.Any())
                    continue;

                var switchNames = GetSwitchNames(info, customSwitchAttrs);
                var switchInfo = AddSwitch(switchNames).SetsField(info);
                switchInfo.WithDescription(string.Join("\n",customSwitchAttrs.Select(x => x.Description ?? "").ToArray()));
            }
        }

        private string[] GetSwitchNames(MemberInfo member, IEnumerable<CommandSwitchAttribute> customAttrs)
        {
            var names = new List<string>();
            foreach (CommandSwitchAttribute attr in customAttrs)
                names.AddRange(attr.Names);

            if (names.Count == 0) 
                names.Add(GetDefaultName(member));

            return names.ToArray();
        }

        private string GetDefaultName(MemberInfo member)
        {
            var name = member.Name;
            StringBuilder formatted = new StringBuilder( "-");

            foreach (char c in name)
            {
                if (c.ToString() == c.ToString().ToUpper())
                    formatted.Append("-");

                formatted.Append(c.ToString().ToLower());
            }

            return formatted.ToString();
        }

        public readonly List<SwitchInfo<T>> Switches = new List<SwitchInfo<T>>();

        public SwitchInfo<T> AddSwitch(params string[] switches)
        {
            var switchInfo = new SwitchInfo<T>(switches);
            Switches.Add(switchInfo);
            return switchInfo;
        }

        public void Parse(string[] args, T settings)
        {
            var switchParams = new List<string>();

            SwitchInfo<T> currentSwitchInfo = null;

            foreach (var arg in args)
            {
                var s = arg;
                var switchInfo = Switches.FirstOrDefault(x => x.Matches(s));

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

        public string GetUsage()
        {
            var switchDescriptions = Switches
                .Select(sw => "[" + string.Join(" | ", sw.Names) + "]");
            return string.Join(" ", switchDescriptions.ToArray());
        }
    }

    public class CommandSwitchAttribute : Attribute
    {
        public CommandSwitchAttribute(params string[] names)
        {
            Names = names;
        }

        public IEnumerable<string> Names { get; private set; }

        public string Description { get; set; }
    }  
}