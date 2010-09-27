using System.Collections.Generic;
using System.Xml;

namespace StorEvil.Configuration
{
    public class XmlDocumentWrapper
    {
        private XmlDocument _doc;
        private XmlNamespaceManager _mgr;

        public XmlDocumentWrapper(XmlDocument doc)
        {
            _doc = doc;
            _mgr = new XmlNamespaceManager(_doc.NameTable);

        }

        public XmlElement SelectElement(string xpath)
        {
            return _doc.SelectSingleNode(xpath, _mgr) as XmlElement;
        }

        public IEnumerable<XmlElement> SelectElements(string xpath)
        {
            foreach (var node in _doc.SelectNodes(xpath, _mgr))
            {
                if (node is XmlElement)
                    yield return (XmlElement)node;
            }
        }

        public void AliasNamespace(string namespaceAlias, string namespaceUrl)
        {
            _mgr.AddNamespace(namespaceAlias, namespaceUrl);
        }
    }
}