using Funq;
using StorEvil.Configuration;
using StorEvil.Core;

namespace StorEvil.Console
{
    internal class HelpContainerConfigurator : IContainerConfigurator
    {
        public string GetUsage()
        {
            return "";
        }

        public void SetupContainer(Container container, ConfigSettings settings, string[] args)
        {
            if (args.Length <= 1)
            {
                container.Register<IStorEvilJob>(new DisplayHelpJob(GetStandardHelpText()));
                return;
            }
            var helpJobFactory = new ContainerConfiguratorFactory(settings).GetConfigurator(args[1]);

            if (helpJobFactory != null)
                container.Register<IStorEvilJob>(
                    new DisplayHelpJob(GetStandardHelpText() + "\r\n\r\nSwitches for '" + args[1] + "': \r\n\r\n" +
                                       helpJobFactory.GetUsage()));
            else
                container.Register<IStorEvilJob>(new DisplayHelpJob(GetStandardHelpText()));
        }

        private string GetStandardHelpText()
        {
            return StandardHelpText + "\r\n\r\nGeneral switches:\r\n\r\n" + new CommonSwitchParser().GetUsage();
        }

        private const string StandardHelpText =
            @"
StorEvil is a natural language BDD framework and runner.
  
Available commands:

 execute - Executes the specs in the console
 init    - Initializes configuration and templates for a StorEvil project
 stub    - Generates example C# method defintions for all unmatched steps in the plaintext steps
 nunit   - Generates NUnit text fixtures (deprecated)

usage:
  'storevil {command} {switches}  - to execute the command
  'storevil help {command}'       - for more information about usage of a command";
    }
}