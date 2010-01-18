using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StorEvil.Console;

namespace StorEvil.Argument_parsing
{

    public class Argument_parsing
    {
        protected ArgParser Parser;

        [SetUp]
        public void SetupContext()
        {
            Parser = new ArgParser();
        }
    }

    [TestFixture]
    public class Building_command_from_args : Argument_parsing
    {
        [Test]
        public void can_create_inplace_job()
        {
            var result = Parser.ParseArguments(new[] {"execute", Assembly.GetExecutingAssembly().Location});
            result.ShouldBeOfType<StorEvilJob>();
            result.ShouldNotBeNull();
        }

        [Test]
        public void can_create_nunit_job()
        {
            var result = Parser.ParseArguments(new[] { "nunit", Assembly.GetExecutingAssembly().Location, Directory.GetCurrentDirectory(), Path.GetTempFileName() });
            result.ShouldBeOfType<StorEvilJob>();
            result.ShouldNotBeNull();
        }

        [Test]
        public void can_create_help_job()
        {
            var result = Parser.ParseArguments(new[] { "help", Assembly.GetExecutingAssembly().Location, Directory.GetCurrentDirectory(), Path.GetTempFileName() });
            result.ShouldBeOfType<DisplayHelpJob>();
            result.ShouldNotBeNull();   
        }

        [Test]
        public void displays_usage_if_not_a_recognized_command()
        {
            var result = Parser.ParseArguments(new[] { "foobar", Assembly.GetExecutingAssembly().Location, Directory.GetCurrentDirectory(), Path.GetTempFileName() });
            result.ShouldBeOfType<DisplayUsageJob>();
            result.ShouldNotBeNull();
        }
    }

    [TestFixture]
    public class Parsing_a_simple_switch_args 
    {
        private SwitchParser<TestConfigSettings> Parser;

        [SetUp]
        public void SetupContext()
        {
            Parser = new SwitchParser<TestConfigSettings>();
            Parser.AddParamSwitch("--foo", "-f").WithAction(c => { c.Foo = true; });

           
        }
        [Test]
        public void sets_setting_using_long_form()
        {
            var settings = new TestConfigSettings();
            var args = new[] {"--foo"};
            Parser.Parse(args, settings);
            settings.Foo.ShouldEqual(true);
        }
    }

    [TestFixture]
    public class Parsing_a_single_param_switch
    {
        private SwitchParser<TestConfigSettings> Parser;
        [SetUp]
        public void SetupContext()
        {
            Parser = new SwitchParser<TestConfigSettings>();
            Parser.AddParamSwitch("--foo", "-f").WithAction(c=> { c.Foo = true;});
        }   
        [Test]
        public void recognizes()
        {
            
        }
    }

    public class SwitchParser<T>
    {
        private List<SwitchInfo<T>> _switchInfos = new List<SwitchInfo<T>>();

        public SwitchInfo<T> AddParamSwitch(params string[] switches)
        {
            var switchInfo = new SwitchInfo<T>(switches);
            _switchInfos.Add(switchInfo);
            return switchInfo;
        }

        public void Parse(string[] args, T settings)
        {
            foreach (var arg in args)
            {
                var switchInfo = _switchInfos.First(x => x.Matches(arg));
                if (switchInfo != null)
                    switchInfo.Execute(settings);
            }
        }
    }

    public class SwitchInfo<T>
    {
        private Action<T> _action;

        public SwitchInfo(string[] switches)
        {
            Switches = switches;
        }

        public void WithAction(Action<T> action)
        {
             _action = action;
        }

        public string[] Switches { get; private set; }

        public bool Matches(string s)
        {
            return Switches.Any(x => x == s);
        }

        public void Execute(T settings)
        {
            _action(settings);
        }
    }

    public class TestConfigSettings
    {
        public bool Foo;
    }
}