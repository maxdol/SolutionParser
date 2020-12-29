using System.Xml;
using System.Collections.Generic;
using System;
using System.Linq;

namespace SolutionParser
{
    class Project
    {
        private string projectPath;

        public Project(string path)
        {
            projectPath = path;
        }

        public IEnumerable<Tuple<string, string, string>> ProjectReferences  // relative path, GUID, condition
        {
            get
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(projectPath);
                XmlNode root = doc.DocumentElement;

                //Create an XmlNamespaceManager for resolving namespaces.
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("def", "http://schemas.microsoft.com/developer/msbuild/2003");

                var list = root.SelectNodes("//def:ItemGroup/def:ProjectReference", nsmgr);
                return list.Cast<XmlNode>().Select(n => Tuple.Create(n.Attributes.GetNamedItem("Include").Value, n.ChildNodes[0].InnerText, n.Attributes.GetNamedItem("Condition")?.Value));
            }
        }
}
}
