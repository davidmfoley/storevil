using NUnit.Framework;

namespace StorEvil.Argument_parsing
{
    public class switch_parsing
    {
        private SwitchParser<TestConfigSettings> Parser;

        private TestConfigSettings TestWithParams(params string[] args)
        {
            var settings = new TestConfigSettings();

            Parser.Parse(args, settings);

            return settings;
        }

        [TestFixture]
        public class Parsing_a_simple_switch_args : switch_parsing
        {
            [SetUp]
            public void SetupContext()
            {
                Parser = new SwitchParser<TestConfigSettings>();
                Parser.AddSwitch("--foo", "-f").WithAction(c => { c.Foo = true; });
            }

            [Test]
            public void sets_setting_using_long_form()
            {
                TestWithParams("--foo").Foo.ShouldEqual(true);
            }

            [Test]
            public void sets_setting_using_short_form()
            {
                TestWithParams("-f").Foo.ShouldEqual(true);
            }
        }

        [TestFixture]
        public class Setting_fields_using_lambdas : switch_parsing
        {
            [SetUp]
            public void SetupContext()
            {
                Parser = new SwitchParser<TestConfigSettings>();
                
            }

            [Test]
            public void sets_boolean_setting_using_long_form()
            {
                Parser.AddSwitch("--foo", "-f").SetsField(c => c.Foo);
                TestWithParams("--foo").Foo.ShouldEqual(true);
            }

            [Test]
            public void sets_boolean_setting_using_short_form()
            {
                Parser.AddSwitch("--foo", "-f").SetsField(c => c.Foo);
                TestWithParams("-f").Foo.ShouldEqual(true);
            }

            [Test]
            public void sets_string_setting_using_short_form()
            {
                Parser.AddSwitch("--bar", "-b").SetsField(c => c.Bar);
                TestWithParams("-b", "bar").Bar.ShouldEqual("bar");
            }

            [Test]
            public void sets_multi_param_setting_using_short_form()
            {
                Parser.AddSwitch("--baz", "-z").SetsField(c => c.Baz);
                TestWithParams("-z", "baz1", "baz2", "baz3").Baz.ElementsShouldEqual("baz1", "baz2", "baz3");
            }
        }

        [TestFixture]
        public class Parsing_a_switch_with_a_parameter : switch_parsing
        {
            [SetUp]
            public void SetupContext()
            {
                Parser = new SwitchParser<TestConfigSettings>();

                Parser.AddSwitch("--bar", "-b").WithSingleParamAction((c, v) => { c.Bar = v; });
            }

            [Test]
            public void sets_param()
            {
                TestWithParams("--bar", "baz").Bar.ShouldEqual("baz");
            }
        }

        [TestFixture]
        public class Parsing_a_switch_with_multiple_parameters : switch_parsing
        {
            private TestConfigSettings Result;

            [SetUp]
            public void SetupContext()
            {
                Parser = new SwitchParser<TestConfigSettings>();

                Parser
                    .AddSwitch("--baz", "-z")
                    .WithMultiParamAction((c, v) => { c.Baz = v; });

                Result = TestWithParams("--baz", "baz1", "baz2", "baz3");
            }

            [Test]
            public void sets_params()
            {
                Result.Baz.ElementsShouldEqual("baz1", "baz2", "baz3");
            }
        }

        public class TestConfigSettings
        {
            public bool Foo;

            public string Bar;
            public string[] Baz;
        }
    }
}