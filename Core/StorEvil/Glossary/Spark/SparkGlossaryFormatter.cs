using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using StorEvil.Core;
using StorEvil.Infrastructure;
using StorEvil.Spark;

namespace StorEvil.Glossary
{
    public class SparkGlossaryFormatter : IGlossaryFormatter
    {
        private readonly ITextWriter _fileWriter;
        private readonly string _pathToTemplateFile;

        public SparkGlossaryFormatter(ITextWriter fileWriter, string pathToTemplateFile)
        {
            _fileWriter = fileWriter;
            _pathToTemplateFile = pathToTemplateFile;
        }

        public void Handle(Glossary glossary)
        {
            var generator = new SparkReportGenerator<GlossaryView>(_fileWriter, _pathToTemplateFile);
            generator.Handle(GetViewModel(glossary));
        }

        private GlossaryViewModel GetViewModel(Glossary glossary)
        {
            var transformed = glossary.Steps.Select(x => BuildViewModelStep(x));
            return new GlossaryViewModel {Steps = transformed};
        }

        private StepViewModel BuildViewModelStep(StepDefinition stepDefinition)
        {
            var describer = new StepDescriber();

            return new StepViewModel
                       {
                           TypeName = stepDefinition.DeclaringType.FullName,
                           MemberName = stepDefinition.Matcher.MemberInfo.Name,
                           Description = describer.Describe(stepDefinition),
                           IsExtensionMethod = IsExtensionMethod(stepDefinition.Matcher.MemberInfo),
                           Children = stepDefinition.Children.Select(BuildViewModelStep)
                       };
        }

        private bool IsExtensionMethod(MemberInfo memberInfo)
        {
            return memberInfo.IsDefined(typeof (ExtensionAttribute), true);          
        }
    }
}