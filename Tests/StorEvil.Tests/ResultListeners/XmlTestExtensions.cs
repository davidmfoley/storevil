using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace StorEvil.ResultListeners
{
    public static class XmlTestExtensions
    {
        public static void ShouldBeValidXml(this string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
        }

        public static IEnumerable<XmlElement> FindElements(this string xml, string xpath)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.SelectNodes(xpath).OfType<XmlElement>();
        }

        public static XmlElement FindElement(this string xml, string xpath)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.SelectNodes(xpath).OfType<XmlElement>().FirstOrDefault();
        }

        public static IEnumerable<XmlElement> FindElements(this XmlElement xmlEl, string xpath)
        {
            return xmlEl.SelectNodes(xpath).OfType<XmlElement>();
        }

        public static XmlElement FindElement(this XmlElement xmlEl, string xpath)
        {
            return xmlEl.SelectNodes(xpath).OfType<XmlElement>().FirstOrDefault();
        }
    }
}