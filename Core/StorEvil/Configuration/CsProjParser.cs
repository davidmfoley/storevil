using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace StorEvil.Configuration
{
    public class CsProjParser
    {
        private XmlDocumentWrapper _doc;

        public CsProjParser(string exampleCsProj)
        {
            var doc = new XmlDocument();
            doc.LoadXml(exampleCsProj);
            _doc = new XmlDocumentWrapper(doc);
            _doc.AliasNamespace("x", "http://schemas.microsoft.com/developer/msbuild/2003");
        }

        public string GetAssemblyLocation()
        {
            var assemblyName = _doc.SelectElement("//x:PropertyGroup/x:AssemblyName").InnerText;
            var path = _doc.SelectElement("//x:PropertyGroup/x:OutputPath").InnerText;
            return Path.Combine(path, assemblyName + ".dll");
        }

        public IEnumerable<string> GetFilesWithTypeNone()
        {
            var nodes = _doc.SelectElements("//x:ItemGroup/x:None");

            foreach (var node in nodes)
            {
                yield return node.GetAttribute("Include");
            }
        }
    }
}