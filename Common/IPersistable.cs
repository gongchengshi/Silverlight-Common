// ////////////////////////////////////////////////////////////////////////////
// COPYRIGHT (c) 2012 Schweitzer Engineering Laboratories, Pullman, WA
// ////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace SEL
{
    public interface IPersistable : IDisposable
    {
        string Persist();
        void Restore(string xml);
    }

    public static class PersistanceHelpers
    {
        public static T Deserialize<T>(string xml)
        {
            var reader = XmlReader.Create(new StringReader(xml));

            reader.Read();

            var name = reader.GetAttribute("Type");

            var obj = DataContractXmlSerializer.FromString(xml, Type.GetType(name));

            return (T)obj;
        }

        public static string Serialize<T>(T obj)
        {
            var xml = DataContractXmlSerializer<T>.ToString(obj);
            var doc = XDocument.Parse(xml);
            doc.Root.SetAttributeValue("Type", typeof(T).AssemblyQualifiedName);
            xml = doc.ToString();

            return xml;
        }
    }
}