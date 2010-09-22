using System;
using System.Collections.Generic;
using System.Xml;
using StorEvil.Core;

namespace StorEvil.Resharper.Tasks
{
    public static class XmlHelper
    {        

        public static TaskScenarioLine[] GetXmlLines(XmlElement element, string elementName)
        {
            var lines = new List<TaskScenarioLine>();

            var bodyElement = (XmlElement)element.GetElementsByTagName(elementName)[0];
            var lineElements = bodyElement.GetElementsByTagName("Line");

            foreach (XmlElement lineElement in lineElements)
            {
                lines.Add(new TaskScenarioLine { Text = lineElement.GetAttribute("Text"), LineNumber = int.Parse(lineElement.GetAttribute("LineNumber")) });
            }

            return lines.ToArray();
        }

        public static TaskScenarioLine[] ConvertLines(ScenarioLine[] lines)
        {
            var body = new TaskScenarioLine[lines.Length];
            for (int i = 0; i < lines.Length; i++)
                body[i] = new TaskScenarioLine { LineNumber = lines[i].LineNumber, Text = lines[i].Text };
            return body;
        }

        private const string MagicDelimiter = "$*$*$";

        public static string[] SplitValues(string text)
        {
            return text.Split(new[] { MagicDelimiter }, StringSplitOptions.None);
        }

        public static string JoinValues(IEnumerable<string> values)
        {
            return string.Join(MagicDelimiter, new List<string>(values).ToArray());
        }


        public static void SetXmlLines(XmlElement element, IEnumerable<TaskScenarioLine> body, string elementName)
        {
            var bodyElement = element.OwnerDocument.CreateElement(elementName);
            element.AppendChild(bodyElement);
            foreach (var line in body)
            {
                var lineElement = element.OwnerDocument.CreateElement("Line");

                SetXmlAttribute(lineElement, "LineNumber", line.LineNumber.ToString());
                SetXmlAttribute(lineElement, "Text", line.Text);
                bodyElement.AppendChild(lineElement);
            }
        }

        private static void SetXmlAttribute(XmlElement e    , string name, string val)
        {
            e.SetAttribute(name, val);
        }

        public static ScenarioLine[] GetScenarioLines(TaskScenarioLine[] inLines)
        {
            var lines = new ScenarioLine[inLines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = new ScenarioLine { Text = inLines[i].Text, LineNumber = inLines[i].LineNumber };
            }

            return lines;
        }
    }
}